using Microsoft.EntityFrameworkCore;
using ShippingRequestSystem.Data;
using ShippingRequestSystem.Models;

namespace ShippingRequestSystem.Services;

public class ApprovalService : IApprovalService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public ApprovalService(IDbContextFactory<AppDbContext> factory)
        => _factory = factory;

    public List<ApprovalStep> BuildApprovalFlow(ShippingRequest request)
    {
        using var db = _factory.CreateDbContext();
        var steps = new List<ApprovalStep>();
        int order = 1;
        var allPerms = db.ApprovalRolePermissions
            .Where(p => p.IsActive)
            .ToList();

        ApprovalStep MakeStep(string roleName, bool required,
                              string? overrideEmail = null,
                              string? overrideName = null)
        {
            var perm = allPerms.FirstOrDefault(p => p.RoleName == roleName);
            return new ApprovalStep
            {
                StepOrder = order++,
                RoleName = roleName,
                ApproverEmail = overrideEmail ?? perm?.UserEmail ?? string.Empty,
                ApproverName = overrideName ?? perm?.UserName ?? string.Empty,
                IsRequired = required
            };
        }

        // Orden de la cadena formal (después de que el solicitante elige cotización
        // y, si aplica, llena Premium Freight):
        // 1) User Manager  2) Sales Manager (si PF es Reembolsable por Cliente)
        // 3) Procurement Manager (si PF es Reembolsable por Proveedor)
        // 4) Finance Manager  5) President  6) Trade Staff (aprobación formal)
        // 7) Trade Manager  8) Trade Coordinator
        steps.Add(new ApprovalStep
        {
            StepOrder = order++,
            RoleName = "User Manager",
            ApproverName = request.UserManagerName,
            ApproverEmail = request.UserManagerEmail,
            IsRequired = true
        });

        // Sales Manager / Procurement Manager: no se sabe si aplican hasta que
        // se llena Premium Freight (ver QuoteService.SavePremiumFreightAsync).
        steps.Add(MakeStep("Sales Manager", false));
        steps.Add(MakeStep("Procurement Manager", false));
        steps.Add(MakeStep("Finance Manager", true));
        steps.Add(MakeStep("President", true));
        steps.Add(MakeStep("Trade Staff", true));
        steps.Add(MakeStep("Trade Manager", true));
        steps.Add(MakeStep("Trade Coordinator", true));

        return steps;
    }

    public async Task<bool> ProcessApprovalAsync(int requestId, int stepId,
        bool approved, string approverEmail, string? comments)
    {
        await using var db = await _factory.CreateDbContextAsync();

        var request = await db.ShippingRequests
            .Include(r => r.ApprovalSteps)
            .FirstOrDefaultAsync(r => r.Id == requestId);

        if (request is null) return false;

        var step = request.ApprovalSteps.FirstOrDefault(s => s.Id == stepId);
        if (step is null || step.IsCompleted) return false;

        if (!string.Equals(step.ApproverEmail, approverEmail,
                           StringComparison.OrdinalIgnoreCase))
            return false;

        step.IsCompleted = true;
        step.IsApproved = approved;
        step.Comments = comments;
        step.ActionDate = DateTime.UtcNow;
        request.UpdatedAt = DateTime.UtcNow;

        if (!approved)
        {
            request.Status = RequestStatus.Rejected;
        }
        else
        {
            var nextStep = request.ApprovalSteps
                .Where(s => !s.IsCompleted && s.IsRequired &&
                            s.StepOrder > step.StepOrder)
                .OrderBy(s => s.StepOrder)
                .FirstOrDefault();

            request.Status = nextStep is null
                ? RequestStatus.Approved
                : nextStep.RoleName switch
                {
                    "User Manager" => RequestStatus.PendingUserManager,
                    "Sales Manager" => RequestStatus.PendingSalesManager,
                    "Procurement Manager" => RequestStatus.PendingProcurementManager,
                    "Finance Manager" => RequestStatus.PendingFinanceManager,
                    "President" => RequestStatus.PendingPresident,
                    "Trade Staff" => RequestStatus.PendingTradeStaffApproval,
                    "Trade Manager" => RequestStatus.PendingTradeManager,
                    "Trade Coordinator" => RequestStatus.PendingTradeCoordinator,
                    _ => request.Status
                };
        }

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<List<ApprovalStep>> GetPendingStepsForApproverAsync(
        string approverEmail)
    {
        await using var db = await _factory.CreateDbContextAsync();

        var requests = await db.ShippingRequests
            .Include(r => r.ApprovalSteps)
            .Where(r => r.Status == RequestStatus.PendingTradeStaffQuotes ||
                        r.Status == RequestStatus.PendingTradeStaffApproval ||
                        r.Status == RequestStatus.PendingTradeCoordinator ||
                        r.Status == RequestStatus.PendingTradeManager ||
                        r.Status == RequestStatus.PendingUserManager ||
                        r.Status == RequestStatus.PendingProcurementManager ||
                        r.Status == RequestStatus.PendingSalesManager ||
                        r.Status == RequestStatus.PendingFinanceManager ||
                        r.Status == RequestStatus.PendingPresident)
            .ToListAsync();

        var pending = new List<ApprovalStep>();

        // PendingTradeStaffQuotes ocurre ANTES de la cadena formal de
        // ApprovalStep (que arranca hasta que el usuario elige cotización) —
        // no hay ningún ApprovalStep todavía para "subir cotizaciones", así
        // que ese caso se resuelve aparte, por quién tiene el rol Trade Staff
        // asignado en /admin/permissions.
        var tradeStaffQuotesRequests = requests
            .Where(r => r.Status == RequestStatus.PendingTradeStaffQuotes)
            .ToList();

        if (tradeStaffQuotesRequests.Count > 0)
        {
            bool isTradeStaff = await db.ApprovalRolePermissions.AnyAsync(p =>
                p.IsActive && p.RoleName == "Trade Staff" &&
                p.UserEmail.ToLower() == approverEmail.ToLower());

            if (isTradeStaff)
            {
                pending.AddRange(tradeStaffQuotesRequests.Select(r => new ApprovalStep
                {
                    ShippingRequestId = r.Id,
                    ShippingRequest = r,
                    RoleName = "Trade Staff",
                    ApproverEmail = approverEmail,
                    IsRequired = true
                }));
            }
        }

        // Para el resto (ya dentro de la cadena formal), el "paso pendiente"
        // real es el de menor StepOrder que todavía no se completa — sin
        // importar cuántos otros roles futuros tenga asignados el mismo
        // aprobador para esa misma solicitud.
        var chainRequests = requests
            .Where(r => r.Status != RequestStatus.PendingTradeStaffQuotes);

        pending.AddRange(chainRequests
            .Select(r => r.ApprovalSteps
                .Where(s => s.IsRequired && !s.IsCompleted)
                .OrderBy(s => s.StepOrder)
                .FirstOrDefault())
            .Where(s => s is not null &&
                        string.Equals(s.ApproverEmail, approverEmail,
                                      StringComparison.OrdinalIgnoreCase))
            .Select(s => s!));

        return pending.OrderBy(s => s.ShippingRequest.CreatedAt).ToList();
    }
}
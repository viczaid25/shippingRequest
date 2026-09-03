using Microsoft.EntityFrameworkCore;
using ShippingRequestSystem.Data;
using ShippingRequestSystem.Models;
using ShippingRequestSystem.ViewModels;

namespace ShippingRequestSystem.Services;

public class ShippingRequestService : IShippingRequestService
{
    private readonly IDbContextFactory<AppDbContext> _factory;
    private readonly IApprovalService _approvalService;
    private readonly FolioService _folioService;


    public ShippingRequestService(
        IDbContextFactory<AppDbContext> factory,
        IApprovalService approvalService,
        FolioService folioService)
    {
        _factory = factory;
        _approvalService = approvalService;
        _folioService = folioService;
    }

    public async Task<ShippingRequest> CreateAsync(ShippingRequestViewModel vm, int? draftId = null)
    {
        await using var db = await _factory.CreateDbContextAsync();

        ShippingRequest request;

        if (draftId is not null)
        {
            request = await db.ShippingRequests
                .Include(r => r.Packages)
                    .ThenInclude(p => p.ChildBoxes)
                        .ThenInclude(b => b.Items)
                .Include(r => r.Packages)
                    .ThenInclude(p => p.Items)
                .FirstOrDefaultAsync(r => r.Id == draftId && r.Status == RequestStatus.Draft)
                ?? throw new Exception("Borrador no encontrado");

            // Reemplazar paquetes/items del borrador con los datos finales del wizard
            db.ShippingPackages.RemoveRange(request.Packages);
            await db.SaveChangesAsync();
            request.Packages.Clear();
        }
        else
        {
            request = new ShippingRequest();
            db.ShippingRequests.Add(request);
        }

        ApplyViewModel(request, vm);
        request.Status = RequestStatus.PendingTradeStaffQuotes;
        request.UpdatedAt = DateTime.UtcNow;

        // Generar folio SR
        request.Folio = await _folioService.GenerateAsync("SR");

        await db.SaveChangesAsync();

        // Paquetes — pasan el db context para mantener la misma conexión
        await SavePackagesAsync(db, vm.Packages, request.Id, null);

        // Flujo de aprobación
        var steps = _approvalService.BuildApprovalFlow(request);
        foreach (var step in steps)
            step.ShippingRequestId = request.Id;

        db.ApprovalSteps.AddRange(steps);
        await db.SaveChangesAsync();

        return request;
    }

    public async Task<ShippingRequest?> GetDraftForUserAsync(string email)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.ShippingRequests
            .Include(r => r.Packages)
                .ThenInclude(p => p.ChildBoxes)
                    .ThenInclude(b => b.Items)
            .Include(r => r.Packages)
                .ThenInclude(p => p.Items)
            .FirstOrDefaultAsync(r => r.RequestorEmail == email && r.Status == RequestStatus.Draft);
    }

    public async Task<int> SaveDraftAsync(ShippingRequestViewModel vm, int? draftId, int currentStep)
    {
        await using var db = await _factory.CreateDbContextAsync();

        ShippingRequest request;

        if (draftId is not null)
        {
            request = await db.ShippingRequests
                .Include(r => r.Packages)
                    .ThenInclude(p => p.ChildBoxes)
                        .ThenInclude(b => b.Items)
                .Include(r => r.Packages)
                    .ThenInclude(p => p.Items)
                .FirstOrDefaultAsync(r => r.Id == draftId && r.Status == RequestStatus.Draft)
                ?? throw new Exception("Borrador no encontrado");

            db.ShippingPackages.RemoveRange(request.Packages);
            await db.SaveChangesAsync();
            request.Packages.Clear();
        }
        else
        {
            request = new ShippingRequest { Status = RequestStatus.Draft, CreatedAt = DateTime.UtcNow };
            db.ShippingRequests.Add(request);
        }

        ApplyViewModel(request, vm);
        request.Status = RequestStatus.Draft;
        request.DraftStep = currentStep;
        request.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        await SavePackagesAsync(db, vm.Packages, request.Id, null);

        return request.Id;
    }

    private static void ApplyViewModel(ShippingRequest request, ShippingRequestViewModel vm)
    {
        request.RequestorName = vm.RequestorName;
        request.RequestorEmail = vm.RequestorEmail;
        request.Department = vm.Department;
        request.BusinessUnit = vm.BusinessUnit;
        request.RequestDate = vm.RequestDate;
        request.Direction = vm.Direction;
        request.Scope = vm.Scope;
        request.NationalCategory = vm.NationalCategory;
        request.BuyerBusinessName = vm.BuyerBusinessName;
        request.BuyerAddress = vm.BuyerAddress;
        request.BuyerTaxId = vm.BuyerTaxId;
        request.RecipientBusinessName = vm.RecipientBusinessName;
        request.RecipientAddress = vm.RecipientAddress;
        request.RecipientTaxId = vm.RecipientTaxId;
        request.RecipientAttentionTo = vm.RecipientAttentionTo;
        request.RecipientPhone = vm.RecipientPhone;
        request.RecipientEmail = vm.RecipientEmail;
        request.ShippingWay = vm.ShippingWay;
        request.ServiceType = vm.ServiceType;
        request.DeliveryDate = vm.DeliveryDate;
        request.ShipmentPurpose = vm.ShipmentPurpose;
        request.InvoiceType = vm.InvoiceType;
        request.RequiresLocalOriginFreight = vm.RequiresLocalOriginFreight;
        request.RequiresLocalDestinationFreight = vm.RequiresLocalDestinationFreight;
        request.RequiresInternationalFreight = vm.RequiresInternationalFreight;
        request.Incoterm = vm.Incoterm;
        // HasCOO / CooDocumentPath: los llena Trade Staff en su revisión, no se sobrescriben aquí.
        request.InvoiceNumber = vm.InvoiceNumber;
        request.OperationType = vm.OperationType;
        request.IsStackable = vm.IsStackable;
        request.PaymentResponsibility = vm.PaymentResponsibility;
        request.ParcelAccount = vm.ParcelAccount;
        request.Carrier = vm.Carrier;
        request.HasInsurance = vm.HasInsurance;
        request.RequestorObservations = vm.RequestorObservations;
        request.MaterialBajaFolio = vm.MaterialBajaFolio;
        request.EvidenceNetWeightPhotoPath = vm.EvidenceNetWeightPhotoPath;
        request.EvidenceGrossWeightPhotoPath = vm.EvidenceGrossWeightPhotoPath;
        request.EvidenceDimensionsPhotoPath = vm.EvidenceDimensionsPhotoPath;
        request.EvidenceFinalPackagingPhotoPath = vm.EvidenceFinalPackagingPhotoPath;
        request.UserManagerName = vm.UserManagerName;
        request.UserManagerEmail = vm.UserManagerEmail;
        if (request.CreatedAt == default) request.CreatedAt = DateTime.UtcNow;
    }

    private static async Task SavePackagesAsync(
        AppDbContext db,
        List<PackageViewModel> packages,
        int requestId,
        int? parentPalletId)
    {
        foreach (var pkgVm in packages)
        {
            var pkg = new ShippingPackage
            {
                ShippingRequestId = requestId,
                PackageType = pkgVm.PackageType,
                Label = pkgVm.Label,
                GrossWeight = pkgVm.GrossWeight,
                Length = pkgVm.Length,
                Width = pkgVm.Width,
                Height = pkgVm.Height,
                ParentPalletId = parentPalletId
            };

            db.ShippingPackages.Add(pkg);
            await db.SaveChangesAsync();

            if (pkgVm.Items.Any())
            {
                foreach (var itemVm in pkgVm.Items)
                {
                    db.ShippingItems.Add(new ShippingItem
                    {
                        ShippingPackageId = pkg.Id,
                        PartNumber = itemVm.PartNumber,
                        Class = itemVm.Class,
                        Description = itemVm.Description,
                        Quantity = itemVm.Quantity,
                        UOM = itemVm.UOM,
                        NetWeight = itemVm.NetWeight,
                        UnitPrice = itemVm.UnitPrice,
                        Amount = itemVm.Amount
                    });
                }
                await db.SaveChangesAsync();
            }

            if (pkgVm.ChildBoxes.Any())
                await SavePackagesAsync(db, pkgVm.ChildBoxes, requestId, pkg.Id);
        }
    }

    public async Task<ShippingRequest?> GetByIdAsync(int id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.ShippingRequests
            .Include(r => r.Packages)
                .ThenInclude(p => p.ChildBoxes)
                    .ThenInclude(b => b.Items)
            .Include(r => r.Packages)
                .ThenInclude(p => p.Items)
            .Include(r => r.PremiumFreightInfo)
            .Include(r => r.ApprovalSteps)
            .Include(r => r.Quotes)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<ShippingRequest>> GetAllAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.ShippingRequests
            .Include(r => r.ApprovalSteps)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ShippingRequest>> GetByEmailAsync(string email)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.ShippingRequests
            .Include(r => r.ApprovalSteps)
            .Where(r => r.RequestorEmail == email)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(ShippingRequest request)
    {
        await using var db = await _factory.CreateDbContextAsync();
        request.UpdatedAt = DateTime.UtcNow;
        db.ShippingRequests.Update(request);
        await db.SaveChangesAsync();
    }
}
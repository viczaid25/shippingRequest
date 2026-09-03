using Microsoft.EntityFrameworkCore;
using ShippingRequestSystem.Data;
using ShippingRequestSystem.Models;

namespace ShippingRequestSystem.Services;

public class QuoteService : IQuoteService
{
    private readonly IDbContextFactory<AppDbContext> _factory;
    private readonly FolioService _folioService;

    public QuoteService(
        IDbContextFactory<AppDbContext> factory,
        FolioService folioService)
    {
        _factory = factory;
        _folioService = folioService;
    }

    public async Task SaveIgiAndQuotesAsync(int requestId, decimal igi, string? incoterm,
                                            OperationType operationType,
                                            bool hasCoo, string? cooDocumentPath,
                                            List<ShippingQuote> quotes)
    {
        await using var db = await _factory.CreateDbContextAsync();

        var request = await db.ShippingRequests
            .FirstOrDefaultAsync(r => r.Id == requestId)
            ?? throw new Exception("Solicitud no encontrada");

        request.IgiPercentage = igi;
        request.Incoterm = incoterm;
        request.OperationType = operationType;
        request.HasCOO = hasCoo;
        request.CooDocumentPath = hasCoo ? cooDocumentPath : null;
        request.UpdatedAt = DateTime.UtcNow;
        request.Status = RequestStatus.PendingUserQuoteSelection;

        // El paso de aprobación "Trade Staff" es la firma formal que ahora ocurre
        // al final de la cadena (después de President) — no se marca aquí; esto
        // solo es la captura inicial de IGI/Incoterm/cotizaciones.

        foreach (var q in quotes)
        {
            q.ShippingRequestId = requestId;
            db.ShippingQuotes.Add(q);
        }

        await db.SaveChangesAsync();
    }

    public async Task<bool> SelectQuoteAsync(int requestId, int quoteId,
                                             string requestorEmail)
    {
        await using var db = await _factory.CreateDbContextAsync();

        var request = await db.ShippingRequests
            .Include(r => r.Quotes)
            .Include(r => r.ApprovalSteps)
            .FirstOrDefaultAsync(r => r.Id == requestId);

        if (request is null) return false;
        if (request.RequestorEmail != requestorEmail) return false;

        foreach (var q in request.Quotes)
            q.IsSelected = false;

        var selected = request.Quotes.FirstOrDefault(q => q.Id == quoteId);
        if (selected is null) return false;

        selected.IsSelected = true;
        request.ApproximateCost = selected.Amount;
        request.UpdatedAt = DateTime.UtcNow;

        bool requiresPremiumFreight =
            request.InvoiceType == InvoiceType.Comercial ||
            request.ApproximateCost >= 5000;

        // Si no requiere Premium Freight, entra directo a la cadena formal —
        // el primer paso siempre es User Manager (ver ApprovalService.BuildApprovalFlow).
        request.Status = requiresPremiumFreight
            ? RequestStatus.PendingPremiumFreight
            : RequestStatus.PendingUserManager;

        await db.SaveChangesAsync();
        return true;
    }

    public async Task SavePremiumFreightAsync(int requestId, PremiumFreightInfo pf)
    {
        await using var db = await _factory.CreateDbContextAsync();

        var request = await db.ShippingRequests
            .Include(r => r.PremiumFreightInfo)
            .Include(r => r.ApprovalSteps)
            .FirstOrDefaultAsync(r => r.Id == requestId)
            ?? throw new Exception("Solicitud no encontrada");

        pf.ShippingRequestId = requestId;

        if (request.PremiumFreightInfo is null)
        {
            pf.Folio = await _folioService.GenerateAsync("PF");
            db.PremiumFreightInfos.Add(pf);
        }
        else
        {
            var e = request.PremiumFreightInfo;
            e.Direction = pf.Direction;
            e.Scope = pf.Scope;
            e.Transport = pf.Transport;
            e.Payment = pf.Payment;
            e.PickUpName = pf.PickUpName;
            e.PickUpAddress = pf.PickUpAddress;
            e.PickUpContact = pf.PickUpContact;
            e.DeliveryName = pf.DeliveryName;
            e.DeliveryAddress = pf.DeliveryAddress;
            e.DeliveryContact = pf.DeliveryContact;
            e.CauseOfShipment = pf.CauseOfShipment;
            e.Countermeasures = pf.Countermeasures;
            e.Forwarder = pf.Forwarder;
        }

        if (pf.Payment == PremiumFreightPayment.ReimbursableByClient)
        {
            var salesStep = request.ApprovalSteps
                .FirstOrDefault(s => s.RoleName == "Sales Manager");
            if (salesStep is not null)
                salesStep.IsRequired = true;
        }

        if (pf.Payment == PremiumFreightPayment.ReimbursableBySupplier)
        {
            var procurementStep = request.ApprovalSteps
                .FirstOrDefault(s => s.RoleName == "Procurement Manager");
            if (procurementStep is not null)
                procurementStep.IsRequired = true;
        }

        // El primer paso de la cadena formal siempre es User Manager
        // (ver ApprovalService.BuildApprovalFlow).
        request.Status = RequestStatus.PendingUserManager;
        request.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
    }
}
using ShippingRequestSystem.Models;

namespace ShippingRequestSystem.Services;

public interface IQuoteService
{
    Task SaveIgiAndQuotesAsync(int requestId, decimal igi, string? incoterm,
                               bool hasCoo, string? cooDocumentPath, List<ShippingQuote> quotes);
    Task<bool> SelectQuoteAsync(int requestId, int quoteId, string requestorEmail);
    Task SavePremiumFreightAsync(int requestId, PremiumFreightInfo pf);
}
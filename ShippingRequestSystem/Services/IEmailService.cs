using ShippingRequestSystem.Models;

namespace ShippingRequestSystem.Services;

public interface IEmailService
{
    Task SendFinanceSummaryAsync(ShippingRequest request);
}

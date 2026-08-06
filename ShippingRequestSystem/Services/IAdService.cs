using ShippingRequestSystem.Models;

namespace ShippingRequestSystem.Services;

public interface IAdService
{
    Task<AdUser?> AuthenticateAsync(string username, string password);
    Task<AdUser?> GetUserAsync(string username);
}
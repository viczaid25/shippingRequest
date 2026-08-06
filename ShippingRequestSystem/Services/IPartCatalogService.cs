using ShippingRequestSystem.Models;

namespace ShippingRequestSystem.Services;

public interface IPartCatalogService
{
    Task<List<PartCatalog>> GetAllActiveAsync();
    Task<List<PartCatalog>> GetAllAsync();
    Task<bool> AddAsync(string partNumber, string? description, string? partClass);
    Task<bool> UpdateAsync(int id, string? description, string? partClass, bool isActive);
}

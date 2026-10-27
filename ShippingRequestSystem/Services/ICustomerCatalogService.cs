using ShippingRequestSystem.Models;

namespace ShippingRequestSystem.Services;

public interface ICustomerCatalogService
{
    Task<List<CustomerCatalog>> GetAllActiveAsync();
    Task<List<CustomerCatalog>> GetAllAsync();
    Task AddAsync(CustomerCatalog customer);
    Task<bool> UpdateAsync(CustomerCatalog customer);
}

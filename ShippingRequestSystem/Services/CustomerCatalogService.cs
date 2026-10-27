using Microsoft.EntityFrameworkCore;
using ShippingRequestSystem.Data;
using ShippingRequestSystem.Models;

namespace ShippingRequestSystem.Services;

public class CustomerCatalogService : ICustomerCatalogService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public CustomerCatalogService(IDbContextFactory<AppDbContext> factory)
        => _factory = factory;

    public async Task<List<CustomerCatalog>> GetAllActiveAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.CustomerCatalogs
            .Where(c => c.IsActive)
            .OrderBy(c => c.BusinessName).ThenBy(c => c.City)
            .ToListAsync();
    }

    public async Task<List<CustomerCatalog>> GetAllAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.CustomerCatalogs
            .OrderBy(c => c.BusinessName).ThenBy(c => c.City)
            .ToListAsync();
    }

    public async Task AddAsync(CustomerCatalog customer)
    {
        await using var db = await _factory.CreateDbContextAsync();
        customer.Id = 0;
        customer.CreatedAt = DateTime.UtcNow;
        db.CustomerCatalogs.Add(customer);
        await db.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(CustomerCatalog customer)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var existing = await db.CustomerCatalogs.FirstOrDefaultAsync(c => c.Id == customer.Id);
        if (existing is null) return false;

        existing.CustomerCode = customer.CustomerCode;
        existing.BusinessName = customer.BusinessName;
        existing.TaxId = customer.TaxId;
        existing.PostalCode = customer.PostalCode;
        existing.Street = customer.Street;
        existing.Neighborhood = customer.Neighborhood;
        existing.City = customer.City;
        existing.State = customer.State;
        existing.Country = customer.Country;
        existing.Currency = customer.Currency;
        existing.IsActive = customer.IsActive;

        await db.SaveChangesAsync();
        return true;
    }
}

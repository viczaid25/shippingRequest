using Microsoft.EntityFrameworkCore;
using ShippingRequestSystem.Data;
using ShippingRequestSystem.Models;

namespace ShippingRequestSystem.Services;

public class PartCatalogService : IPartCatalogService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public PartCatalogService(IDbContextFactory<AppDbContext> factory)
        => _factory = factory;

    public async Task<List<PartCatalog>> GetAllActiveAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.PartCatalogs
            .Where(p => p.IsActive)
            .OrderBy(p => p.PartNumber)
            .ToListAsync();
    }

    public async Task<List<PartCatalog>> GetAllAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.PartCatalogs
            .OrderBy(p => p.PartNumber)
            .ToListAsync();
    }

    public async Task<bool> AddAsync(string partNumber, string? description, string? partClass)
    {
        await using var db = await _factory.CreateDbContextAsync();

        if (await db.PartCatalogs.AnyAsync(p => p.PartNumber == partNumber))
            return false;

        db.PartCatalogs.Add(new PartCatalog
        {
            PartNumber = partNumber,
            Description = description,
            Class = partClass,
            IsActive = true
        });

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(int id, string? description, string? partClass, bool isActive)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var part = await db.PartCatalogs.FirstOrDefaultAsync(p => p.Id == id);
        if (part is null) return false;

        part.Description = description;
        part.Class = partClass;
        part.IsActive = isActive;

        await db.SaveChangesAsync();
        return true;
    }
}

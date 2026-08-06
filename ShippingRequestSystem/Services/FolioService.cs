using Microsoft.EntityFrameworkCore;
using ShippingRequestSystem.Data;

namespace ShippingRequestSystem.Services;

public class FolioService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public FolioService(IDbContextFactory<AppDbContext> factory)
        => _factory = factory;

    /// <summary>
    /// Genera el siguiente folio para el prefijo dado.
    /// Usa una transacción con bloqueo para evitar duplicados concurrentes.
    /// Ejemplo: SR-2026-0001, PF-2026-0001
    /// </summary>
    public async Task<string> GenerateAsync(string prefix)
    {
        await using var db = await _factory.CreateDbContextAsync();

        // EnableRetryOnFailure (Program.cs) exige que las transacciones manuales se
        // ejecuten a través de CreateExecutionStrategy — de lo contrario EF Core lanza
        // "does not support user-initiated transactions" al primer SaveChanges.
        var strategy = db.Database.CreateExecutionStrategy();
        int lastNumber = 0;
        int year = DateTime.UtcNow.Year;

        await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await db.Database
                .BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            var counter = await db.FolioCounters
                .FirstOrDefaultAsync(f => f.Prefix == prefix && f.Year == year);

            if (counter is null)
            {
                counter = new Models.FolioCounter
                {
                    Prefix = prefix,
                    Year = year,
                    LastNumber = 1
                };
                db.FolioCounters.Add(counter);
            }
            else
            {
                counter.LastNumber++;
                db.FolioCounters.Update(counter);
            }

            await db.SaveChangesAsync();
            await tx.CommitAsync();

            lastNumber = counter.LastNumber;
        });

        return $"{prefix}-{year}-{lastNumber:D4}";
    }
}
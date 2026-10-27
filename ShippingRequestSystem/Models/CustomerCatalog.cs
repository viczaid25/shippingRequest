using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShippingRequestSystem.Models;

/// <summary>
/// Catálogo de clientes/destinos de Trade. Un mismo cliente puede aparecer varias
/// veces (un registro por domicilio), por eso CustomerCode no es único.
/// </summary>
[Table("prcPurchaseCustomerCatalog")]
public class CustomerCatalog
{
    [Key]
    public int Id { get; set; }

    [MaxLength(50)]
    public string? CustomerCode { get; set; }

    [Required, MaxLength(300)]
    public string BusinessName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? TaxId { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }

    [MaxLength(300)]
    public string? Street { get; set; }

    [MaxLength(200)]
    public string? Neighborhood { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [MaxLength(10)]
    public string? Currency { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Domicilio en una línea, para autollenar la solicitud.</summary>
    [NotMapped]
    public string FullAddress => string.Join(", ", new[]
        { Street, Neighborhood, City, State, PostalCode, Country }
        .Where(s => !string.IsNullOrWhiteSpace(s)));
}

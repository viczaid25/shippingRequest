using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShippingRequestSystem.Models;

[Table("prcPurchaseShippingQuote")]
public class ShippingQuote
{
    [Key]
    public int Id { get; set; }

    public int ShippingRequestId { get; set; }
    public ShippingRequest ShippingRequest { get; set; } = null!;

    [Required, MaxLength(300)]
    public string ProviderName { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    // Ruta relativa del PDF guardado en el servidor
    [MaxLength(500)]
    public string? FilePath { get; set; }

    // Nombre original del archivo para mostrar al usuario
    [MaxLength(200)]
    public string? FileName { get; set; }

    public bool IsSelected { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
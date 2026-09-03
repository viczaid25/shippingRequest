using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShippingRequestSystem.Models;

[Table("prcPurchaseShippingItem")]
public class ShippingItem
{
    [Key]
    public int Id { get; set; }

    public int ShippingPackageId { get; set; }
    public ShippingPackage ShippingPackage { get; set; } = null!;

    [Required, MaxLength(200)]
    public string PartNumber { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Class { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public int Quantity { get; set; }

    [MaxLength(50)]
    public string? UOM { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal NetWeight { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
}
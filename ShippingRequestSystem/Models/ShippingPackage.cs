using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShippingRequestSystem.Models;

[Table("prcPurchaseShippingPackage")]
public class ShippingPackage
{
    [Key]
    public int Id { get; set; }

    public int ShippingRequestId { get; set; }
    public ShippingRequest ShippingRequest { get; set; } = null!;

    public PackageType PackageType { get; set; }

    // If it's a Box inside a Pallet
    public int? ParentPalletId { get; set; }
    public ShippingPackage? ParentPallet { get; set; }

    [MaxLength(100)]
    public string? Label { get; set; }

    // Peso y dimensiones del pallet completo (armado), solo relevante si es Pallet
    [Column(TypeName = "decimal(18,4)")]
    public decimal GrossWeight { get; set; }
    [Column(TypeName = "decimal(18,4)")]
    public decimal Length { get; set; }
    [Column(TypeName = "decimal(18,4)")]
    public decimal Width { get; set; }
    [Column(TypeName = "decimal(18,4)")]
    public decimal Height { get; set; }

    public ICollection<ShippingPackage> ChildBoxes { get; set; } = new List<ShippingPackage>();
    public ICollection<ShippingItem> Items { get; set; } = new List<ShippingItem>();
}
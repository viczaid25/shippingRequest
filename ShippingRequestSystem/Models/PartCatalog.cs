using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShippingRequestSystem.Models;

[Table("prcPurchasePartCatalog")]
public class PartCatalog
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string PartNumber { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? Class { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

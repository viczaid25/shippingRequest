using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShippingRequestSystem.Models;

[Table("prcPurchaseFolioCounter")]
public class FolioCounter
{
    [Key]
    public int Id { get; set; }

    // "SR" o "PF"
    [Required, MaxLength(10)]
    public string Prefix { get; set; } = string.Empty;

    public int Year { get; set; }

    public int LastNumber { get; set; } = 0;
}
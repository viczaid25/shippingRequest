using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShippingRequestSystem.Models;

[Table("prcPurchaseSystemAdmin")]
public class SystemAdmin
{
    [Key]
    public int Id { get; set; }

    public int MeaxUserId { get; set; }

    [MaxLength(100)]
    public string? PcLoginId { get; set; }

    [Required, MaxLength(200)]
    public string UserName { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string UserEmail { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? UserDepartment { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
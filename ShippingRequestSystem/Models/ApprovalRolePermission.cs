using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShippingRequestSystem.Models;

[Table("prcPurchaseApprovalRolePermission")]
public class ApprovalRolePermission
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string RoleName { get; set; } = string.Empty;

    public int MeaxUserId { get; set; }

    [MaxLength(100)]
    public string? PcLoginId { get; set; }

    [MaxLength(200)]
    public string UserName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string UserEmail { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? UserDepartment { get; set; }

    [MaxLength(200)]
    public string? UserPosition { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(200)]
    public string? CreatedBy { get; set; }
}
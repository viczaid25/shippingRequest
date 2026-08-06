using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShippingRequestSystem.Models;

[Table("prcPurchaseApprovalStep")]
public class ApprovalStep
{
    [Key]
    public int Id { get; set; }

    public int ShippingRequestId { get; set; }
    public ShippingRequest ShippingRequest { get; set; } = null!;

    public int StepOrder { get; set; }

    [MaxLength(100)]
    public string RoleName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? ApproverName { get; set; }

    [MaxLength(200)]
    public string? ApproverEmail { get; set; }

    public bool IsRequired { get; set; } = true;
    public bool IsCompleted { get; set; } = false;
    public bool? IsApproved { get; set; }

    [MaxLength(1000)]
    public string? Comments { get; set; }

    public DateTime? ActionDate { get; set; }
}
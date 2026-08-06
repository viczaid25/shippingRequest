using ShippingRequestSystem.Models;

namespace ShippingRequestSystem.Services;

public interface IApprovalService
{
    List<ApprovalStep> BuildApprovalFlow(ShippingRequest request);
    Task<bool> ProcessApprovalAsync(int requestId, int stepId, bool approved,
                                    string approverEmail, string? comments);
    Task<List<ApprovalStep>> GetPendingStepsForApproverAsync(string approverEmail);
}
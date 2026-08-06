using ShippingRequestSystem.Models;
using ShippingRequestSystem.ViewModels;

namespace ShippingRequestSystem.Services;

public interface IShippingRequestService
{
    Task<ShippingRequest> CreateAsync(ShippingRequestViewModel vm, int? draftId = null);
    Task<ShippingRequest?> GetByIdAsync(int id);
    Task<List<ShippingRequest>> GetAllAsync();
    Task<List<ShippingRequest>> GetByEmailAsync(string email);
    Task UpdateAsync(ShippingRequest request);

    // Borradores (autoguardado del wizard de Nueva Solicitud)
    Task<ShippingRequest?> GetDraftForUserAsync(string email);
    Task<int> SaveDraftAsync(ShippingRequestViewModel vm, int? draftId, int currentStep);
}
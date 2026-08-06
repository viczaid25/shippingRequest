using ShippingRequestSystem.Models;

namespace ShippingRequestSystem.Services;

public interface IPermissionService
{
    // Usuarios
    Task<List<MeaxUser>> SearchUsersAsync(string query);
    Task<MeaxUser?> GetUserByLoginAsync(string loginId);

    // Permisos por rol
    Task<List<ApprovalRolePermission>> GetPermissionsByRoleAsync(string roleName);
    Task<List<ApprovalRolePermission>> GetAllPermissionsAsync();
    Task<bool> AddPermissionAsync(string roleName,
                                           MeaxUser user, string createdBy);
    Task<bool> RemovePermissionAsync(int permissionId);
    Task<bool> UserHasRoleAsync(string email, string roleName);
    Task<string?> GetRoleByEmailAsync(string email);

    // Admins
    Task<List<SystemAdmin>> GetAdminsAsync();
    Task<bool> AddAdminAsync(MeaxUser user);
    Task<bool> RemoveAdminAsync(int adminId);
    Task<bool> IsAdminAsync(string email);
}
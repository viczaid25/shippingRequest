using ShippingRequestSystem.Models;

namespace ShippingRequestSystem.Services;

public interface IPermissionService
{
    // Usuarios
    Task<List<MeaxUser>> SearchUsersAsync(string query);
    Task<MeaxUser?> GetUserByLoginAsync(string loginId);

    // Lista navegable de usuarios: sin texto (o menos de 2 caracteres) devuelve
    // los primeros "take" usuarios por nombre; con texto filtra igual que
    // SearchUsersAsync. Pensado para selectores tipo "lista desplegable con
    // buscador" (ej. Manager/Supervisor en Nueva Solicitud).
    Task<List<MeaxUser>> ListUsersAsync(string? query, int take = 30, string[]? positions = null);

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
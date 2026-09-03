using Microsoft.EntityFrameworkCore;
using ShippingRequestSystem.Data;
using ShippingRequestSystem.Models;

namespace ShippingRequestSystem.Services;

public class PermissionService : IPermissionService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public PermissionService(IDbContextFactory<AppDbContext> factory)
        => _factory = factory;

    // ── Usuarios ──────────────────────────────────────────────────────────────

    public async Task<List<MeaxUser>> SearchUsersAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return new List<MeaxUser>();

        query = query.Trim().ToLower();

        await using var db = await _factory.CreateDbContextAsync();
        return await db.MeaxUsers
            .Where(u =>
                (u.Username != null && u.Username.ToLower().Contains(query)) ||
                (u.Email != null && u.Email.ToLower().Contains(query)) ||
                (u.PcLoginId != null && u.PcLoginId.ToLower().Contains(query)))
            .OrderBy(u => u.Username)
            .Take(20)
            .ToListAsync();
    }

    public async Task<List<MeaxUser>> ListUsersAsync(string? query, int take = 30, string[]? positions = null)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var q = db.MeaxUsers.AsQueryable();

        if (positions is { Length: > 0 })
            q = q.Where(u => u.Position != null && positions.Contains(u.Position));

        var term = query?.Trim().ToLower();
        if (!string.IsNullOrEmpty(term) && term.Length >= 2)
        {
            q = q.Where(u =>
                (u.Username != null && u.Username.ToLower().Contains(term)) ||
                (u.Email != null && u.Email.ToLower().Contains(term)) ||
                (u.PcLoginId != null && u.PcLoginId.ToLower().Contains(term)));
        }

        return await q.OrderBy(u => u.Username).Take(take).ToListAsync();
    }

    public async Task<MeaxUser?> GetUserByLoginAsync(string loginId)
    {
        if (string.IsNullOrWhiteSpace(loginId)) return null;

        var login = loginId.Trim().ToLower();

        await using var db = await _factory.CreateDbContextAsync();
        return await db.MeaxUsers
            .FirstOrDefaultAsync(u =>
                (u.PcLoginId != null && u.PcLoginId.ToLower() == login) ||
                (u.Username != null && u.Username.ToLower() == login));
    }

    // ── Permisos ──────────────────────────────────────────────────────────────

    public async Task<List<ApprovalRolePermission>> GetPermissionsByRoleAsync(
        string roleName)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.ApprovalRolePermissions
            .Where(p => p.RoleName == roleName && p.IsActive)
            .OrderBy(p => p.UserName)
            .ToListAsync();
    }

    public async Task<List<ApprovalRolePermission>> GetAllPermissionsAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.ApprovalRolePermissions
            .Where(p => p.IsActive)
            .OrderBy(p => p.RoleName)
            .ThenBy(p => p.UserName)
            .ToListAsync();
    }

    public async Task<bool> AddPermissionAsync(string roleName,
        MeaxUser user, string createdBy)
    {
        await using var db = await _factory.CreateDbContextAsync();

        bool exists = await db.ApprovalRolePermissions
            .AnyAsync(p => p.RoleName == roleName &&
                           p.MeaxUserId == user.Id &&
                           p.IsActive);
        if (exists) return false;

        db.ApprovalRolePermissions.Add(new ApprovalRolePermission
        {
            RoleName = roleName,
            MeaxUserId = user.Id,
            PcLoginId = user.PcLoginId,
            UserName = user.Username ?? user.PcLoginId ?? string.Empty,
            UserEmail = user.Email ?? string.Empty,
            UserDepartment = user.Department,
            UserPosition = user.Position,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        });

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemovePermissionAsync(int permissionId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var perm = await db.ApprovalRolePermissions.FindAsync(permissionId);
        if (perm is null) return false;

        perm.IsActive = false;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UserHasRoleAsync(string email, string roleName)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.ApprovalRolePermissions
            .AnyAsync(p => p.UserEmail.ToLower() == email.ToLower() &&
                           p.RoleName == roleName &&
                           p.IsActive);
    }

    public async Task<string?> GetRoleByEmailAsync(string email)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var perm = await db.ApprovalRolePermissions
            .Where(p => p.UserEmail.ToLower() == email.ToLower() && p.IsActive)
            .FirstOrDefaultAsync();
        return perm?.RoleName;
    }

    // ── Admins ────────────────────────────────────────────────────────────────

    public async Task<List<SystemAdmin>> GetAdminsAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.SystemAdmins
            .Where(a => a.IsActive)
            .OrderBy(a => a.UserName)
            .ToListAsync();
    }

    public async Task<bool> AddAdminAsync(MeaxUser user)
    {
        await using var db = await _factory.CreateDbContextAsync();

        bool exists = await db.SystemAdmins
            .AnyAsync(a => a.MeaxUserId == user.Id && a.IsActive);
        if (exists) return false;

        db.SystemAdmins.Add(new SystemAdmin
        {
            MeaxUserId = user.Id,
            PcLoginId = user.PcLoginId,
            UserName = user.Username ?? user.PcLoginId ?? string.Empty,
            UserEmail = user.Email ?? string.Empty,
            UserDepartment = user.Department,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveAdminAsync(int adminId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var admin = await db.SystemAdmins.FindAsync(adminId);
        if (admin is null) return false;

        admin.IsActive = false;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsAdminAsync(string email)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.SystemAdmins
            .AnyAsync(a => a.UserEmail.ToLower() == email.ToLower() &&
                           a.IsActive);
    }
}
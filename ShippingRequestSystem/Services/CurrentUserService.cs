using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace ShippingRequestSystem.Services;

public class CurrentUserService
{
    private readonly AuthenticationStateProvider _authProvider;
    private ClaimsPrincipal? _user;

    public CurrentUserService(AuthenticationStateProvider authProvider)
        => _authProvider = authProvider;

    private async Task<ClaimsPrincipal> GetUserAsync()
    {
        if (_user is not null) return _user;
        var state = await _authProvider.GetAuthenticationStateAsync();
        _user = state.User;
        return _user;
    }

    public async Task<bool> IsAuthenticatedAsync()
        => (await GetUserAsync()).Identity?.IsAuthenticated == true;

    public async Task<string> GetUsernameAsync()
        => (await GetUserAsync()).FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

    public async Task<string> GetEmailAsync()
        => (await GetUserAsync()).FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

    public async Task<string> GetDisplayNameAsync()
        => (await GetUserAsync()).FindFirst("DisplayName")?.Value ?? string.Empty;

    public async Task<string> GetDepartmentAsync()
        => (await GetUserAsync()).FindFirst("Department")?.Value ?? string.Empty;

    public async Task<List<string>> GetRolesAsync()
    {
        var user = await GetUserAsync();
        return user.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();
    }

    public async Task<bool> IsInRoleAsync(string role)
    {
        var roles = await GetRolesAsync();
        return roles.Any(r => r.Contains(role, StringComparison.OrdinalIgnoreCase));
    }

    // Propiedades síncronas
    public string Username => GetUsernameAsync().GetAwaiter().GetResult();
    public string Email => GetEmailAsync().GetAwaiter().GetResult();
    public string DisplayName => GetDisplayNameAsync().GetAwaiter().GetResult();
    public string Department => GetDepartmentAsync().GetAwaiter().GetResult();

    // Los roles de aprobación (Trade Staff, Trade Coordinator, Finance Manager, etc.)
    // NO se determinan por grupo de AD — se asignan por persona en /admin/permissions
    // (tabla ApprovalRolePermission) y se verifican comparando el email del usuario
    // contra el ApproverEmail del ApprovalStep correspondiente en cada solicitud.

    // IsAdmin NO está aquí — se verifica contra la BD via IPermissionService
}
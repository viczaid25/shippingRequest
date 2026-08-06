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

    public bool IsTradeStaff
        => IsInRoleAsync("Trade_Staff").GetAwaiter().GetResult();
    public bool IsTradeCoordinator
        => IsInRoleAsync("Trade_Coordinator").GetAwaiter().GetResult();
    public bool IsTradeManager
        => IsInRoleAsync("Trade_Manager").GetAwaiter().GetResult();
    public bool IsProcurement
        => IsInRoleAsync("Procurement_Manager").GetAwaiter().GetResult();
    public bool IsSalesManager
        => IsInRoleAsync("Sales_Manager").GetAwaiter().GetResult();
    public bool IsFinanceManager
        => IsInRoleAsync("Finance_Manager").GetAwaiter().GetResult();
    public bool IsPresident
        => IsInRoleAsync("President").GetAwaiter().GetResult();

    // IsAdmin NO está aquí — se verifica contra la BD via IPermissionService
}
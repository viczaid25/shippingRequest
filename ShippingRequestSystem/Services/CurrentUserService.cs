using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using ShippingRequestSystem.Auth;

namespace ShippingRequestSystem.Services;

public class CurrentUserService
{
    private readonly AuthenticationStateProvider _authProvider;
    private readonly IPermissionService _permissionService;
    private ClaimsPrincipal? _user;
    private string? _resolvedEmail;
    private bool _emailResolved;

    public CurrentUserService(AuthenticationStateProvider authProvider, IPermissionService permissionService)
    {
        _authProvider = authProvider;
        _permissionService = permissionService;
    }

    private async Task<ClaimsPrincipal> GetUserAsync()
    {
        if (_user is not null) return _user;
        var state = await _authProvider.GetAuthenticationStateAsync();
        _user = state.User;
        return _user;
    }

    public async Task<bool> IsAuthenticatedAsync()
        => (await GetUserAsync()).Identity?.IsAuthenticated == true;

    // PcLoginId (login de red) — viene del claim del hub, nunca de AD directo.
    public async Task<string> GetUsernameAsync()
        => (await GetUserAsync()).FindFirst(MeaxClaims.PcLoginId)?.Value ?? string.Empty;

    public async Task<string> GetDisplayNameAsync()
        => (await GetUserAsync()).FindFirst(MeaxClaims.DisplayName)?.Value ?? string.Empty;

    public async Task<string> GetDepartmentAsync()
        => (await GetUserAsync()).FindFirst(MeaxClaims.Department)?.Value ?? string.Empty;

    public async Task<string> GetPositionAsync()
        => (await GetUserAsync()).FindFirst(MeaxClaims.Position)?.Value ?? string.Empty;

    // Nómina del empleado (4-5 dígitos). Puede venir vacía si AD no la tiene registrada.
    public async Task<string> GetEmployeeIdAsync()
        => (await GetUserAsync()).FindFirst(MeaxClaims.EmployeeId)?.Value ?? string.Empty;

    // Solo presente en tokens SSO; rol del usuario en este sistema.
    public async Task<string> GetSystemRoleAsync()
        => (await GetUserAsync()).FindFirst(MeaxClaims.SystemRole)?.Value ?? string.Empty;

    // El JWT del hub NO trae email — se resuelve una sola vez por circuito
    // contra meax_all_user (BD propia de esta app, no la BD del hub) usando el
    // PcLoginId del claim. Todo el sistema de aprobaciones compara por email
    // (ApprovalStep.ApproverEmail), de ahí que se siga resolviendo aquí.
    public async Task<string> GetEmailAsync()
    {
        if (_emailResolved) return _resolvedEmail ?? string.Empty;

        var login = await GetUsernameAsync();
        if (!string.IsNullOrWhiteSpace(login))
        {
            var meaxUser = await _permissionService.GetUserByLoginAsync(login);
            _resolvedEmail = meaxUser?.Email;
        }

        _emailResolved = true;
        return _resolvedEmail ?? string.Empty;
    }

    // Propiedades síncronas
    public string Username => GetUsernameAsync().GetAwaiter().GetResult();

    // GetEmailAsync hace una consulta real a BD (a diferencia de las demás,
    // que solo leen claims y completan sin ceder el hilo). Si se bloquea con
    // GetAwaiter().GetResult() directo desde un OnInitializedAsync (ej.
    // NavMenu), la continuación necesita el mismo SynchronizationContext de
    // Blazor que el llamador tiene bloqueado — deadlock. Task.Run la separa
    // del contexto capturado y evita el problema sin tocar cada consumidor.
    public string Email => Task.Run(() => GetEmailAsync()).GetAwaiter().GetResult();

    public string DisplayName => GetDisplayNameAsync().GetAwaiter().GetResult();
    public string Department => GetDepartmentAsync().GetAwaiter().GetResult();
    public string Position => GetPositionAsync().GetAwaiter().GetResult();
    public string EmployeeId => GetEmployeeIdAsync().GetAwaiter().GetResult();
    public string SystemRole => GetSystemRoleAsync().GetAwaiter().GetResult();

    // Los roles de aprobación (Trade Staff, Trade Coordinator, Finance Manager, etc.)
    // NO se determinan por SystemRole ni por grupo de AD — se asignan por persona en
    // /admin/permissions (tabla ApprovalRolePermission) y se verifican comparando el
    // email del usuario (resuelto arriba) contra el ApproverEmail del ApprovalStep
    // correspondiente en cada solicitud.

    // IsAdmin NO está aquí — se verifica contra la BD via IPermissionService
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ShippingRequestSystem.Auth;

namespace ShippingRequestSystem.Controllers;

/// <summary>
/// Login SOLO para modo desarrollo (sin Meax One). En producción esta acción
/// no hace nada útil: cualquier request sin token válido ya fue redirigido al
/// SSO del hub antes de llegar aquí. Emite un ClaimsPrincipal con exactamente
/// los mismos claims que el JWT del hub (ver MeaxClaims).
/// </summary>
[Route("[controller]")]
[AllowAnonymous]
public class AuthController : Controller
{
    private readonly AuthMode _authMode;
    private readonly IConfiguration _config;

    public AuthController(AuthMode authMode, IConfiguration config)
    {
        _authMode = authMode;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromForm] string username,
        [FromForm] string password,
        [FromForm] string? returnUrl)
    {
        if (!_authMode.DevLoginActivo)
        {
            // En producción el login lo hace el hub; ir a inicio dispara el SSO.
            return Redirect(Url.Content("~/"));
        }

        var appRoot = Url.Content("~/");
        var loginPath = Url.Content("~/login");

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return Redirect($"{loginPath}?error={Uri.EscapeDataString("Usuario y contraseña son requeridos")}" +
                             $"&returnUrl={Uri.EscapeDataString(returnUrl ?? appRoot)}");

        var devPassword = _config["Auth:DevPassword"] ?? "dev";
        if (!string.Equals(password, devPassword, StringComparison.Ordinal))
            return Redirect($"{loginPath}?error={Uri.EscapeDataString("Contraseña de desarrollo incorrecta")}" +
                             $"&returnUrl={Uri.EscapeDataString(returnUrl ?? appRoot)}");

        var info = ConstruirHubUserInfo(username.Trim());
        var identity = MeaxClaims.CrearIdentidad(CookieAuthenticationDefaults.AuthenticationScheme, info);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            });

        // "/" también se normaliza a "~/": es el valor por defecto que pone
        // Login.razor cuando no venía returnUrl en la query.
        var redirect = string.IsNullOrEmpty(returnUrl) || returnUrl == "/" ? appRoot : returnUrl;
        return Redirect(redirect);
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (_authMode.DevLoginActivo)
        {
            return Redirect(Url.Content("~/login"));
        }

        // En producción la sesión pertenece al hub: se elimina la cookie
        // .MEAX.JWT de este host y se regresa a Meax One.
        Response.Cookies.Delete(".MEAX.JWT");

        var hubBaseUrl = (_config["Hub:BaseUrl"] ?? string.Empty).TrimEnd('/');
        return Redirect(string.IsNullOrWhiteSpace(hubBaseUrl) ? Url.Content("~/") : hubBaseUrl);
    }

    /// <summary>
    /// Si el usuario tecleado es el de Auth:DevUser se usa su perfil completo;
    /// con otro usuario se conserva el SystemRole de config pero sin nómina ni
    /// puesto (evita matches falsos contra Tress/Expedientes).
    /// </summary>
    private HubUserInfo ConstruirHubUserInfo(string username)
    {
        var devUser = _config.GetSection("Auth:DevUser");
        var configLogin = devUser["PcLoginId"] ?? string.Empty;

        if (string.Equals(username, configLogin, StringComparison.OrdinalIgnoreCase))
        {
            return new HubUserInfo(
                configLogin,
                devUser["DisplayName"],
                devUser["Department"],
                devUser["Position"],
                devUser["EmployeeId"],
                devUser["SystemRole"]);
        }

        return new HubUserInfo(
            username,
            username,
            devUser["Department"],
            null,
            null,
            devUser["SystemRole"]);
    }
}

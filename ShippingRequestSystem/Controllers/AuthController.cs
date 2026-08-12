using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ShippingRequestSystem.Services;

namespace ShippingRequestSystem.Controllers;

[Route("[controller]")]
public class AuthController : Controller
{
    private readonly IAdService _ad;

    public AuthController(IAdService ad) => _ad = ad;

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromForm] string username,
        [FromForm] string password,
        [FromForm] string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
            return Redirect($"/login?error={Uri.EscapeDataString("Credenciales inválidas")}");

        var user = await _ad.AuthenticateAsync(username.Trim(), password);

        if (user is null)
            return Redirect($"/login?error={Uri.EscapeDataString("Usuario o contraseña incorrectos")}");

        // Construir claims
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name,  user.Username),
            new(ClaimTypes.Email, user.Email),
            new("DisplayName",    user.DisplayName),
            new("Department",     user.Department),
            new("Title",          user.Title)
        };

        // Agregar roles desde grupos AD
        foreach (var group in user.Groups)
            claims.Add(new Claim(ClaimTypes.Role, group));

        var identity = new ClaimsIdentity(claims,
                            CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            });

        var redirect = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
        return Redirect(redirect);
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/login");
    }
}
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace ShippingRequestSystem.Middleware;

/// <summary>
/// Exige autenticación en toda la app, salvo excepciones deliberadas.
/// No se implementa con RequireAuthorization()/una política de fallback sobre
/// MapRazorComponents porque eso también protege los endpoints internos de
/// Blazor Interactive Server (p. ej. _framework/blazor.web.js): el script
/// nunca carga y la app queda colgada antes de poder mostrar el login o
/// redirigir al hub. En vez de eso, este middleware corre después de
/// UseAuthorization() y dispara el challenge del esquema de auth activo
/// (que ya sabe redirigir al SSO del hub o al login de desarrollo).
/// </summary>
public class RequireAuthMiddleware
{
    private readonly RequestDelegate _next;

    // Infraestructura de Blazor/estáticos que debe quedar accesible sin
    // autenticar (los estáticos de wwwroot ya se sirven antes de este
    // middleware vía UseStaticFiles; se listan aquí también como respaldo).
    private static readonly string[] PublicPrefixes =
    {
        "/_framework", "/_blazor", "/login", "/auth", "/css", "/js", "/favicon"
    };

    public RequireAuthMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        bool isPublicPrefix = PublicPrefixes.Any(p =>
            path.StartsWith(p, StringComparison.OrdinalIgnoreCase));

        bool isAllowAnonymous =
            context.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() is not null;

        if (!isPublicPrefix && !isAllowAnonymous && context.User.Identity?.IsAuthenticated != true)
        {
            await context.ChallengeAsync();
            return;
        }

        await _next(context);
    }
}

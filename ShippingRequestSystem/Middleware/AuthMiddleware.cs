namespace ShippingRequestSystem.Middleware;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    // Rutas públicas que no requieren login
    private static readonly string[] PublicPaths =
    {
        "/login", "/auth/login", "/auth/logout",
        "/_blazor", "/_framework", "/css", "/js",
        "/favicon"
    };

    public AuthMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        bool isPublic = PublicPaths.Any(p =>
            path.StartsWith(p, StringComparison.OrdinalIgnoreCase));

        if (!isPublic && context.User?.Identity?.IsAuthenticated != true)
        {
            var returnUrl = Uri.EscapeDataString(path);
            context.Response.Redirect($"/login?returnUrl={returnUrl}");
            return;
        }

        await _next(context);
    }
}
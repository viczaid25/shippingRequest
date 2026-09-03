using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;

namespace ShippingRequestSystem.Auth;

/// <summary>
/// Autenticación de producción contra meaxHub (Meax One):
/// - Valida el JWT HS256 emitido por el hub (cookie .MEAX.JWT o query ?meax_token=).
/// - Si no hay token válido, redirige al SSO del hub en lugar de responder 401.
/// - Al validar un token recibido por query (deep-link, dura 5 min) lo convierte
///   en una cookie de sesión local para que el circuito de Blazor Server y las
///   peticiones siguientes queden autenticadas sin depender del token efímero.
/// </summary>
public static class HubAuthExtensions
{
    public const string SchemeName = "MeaxHub";
    private const string HubJwtCookie = ".MEAX.JWT";
    private const string TokenQueryParam = "meax_token";
    private const string RoleQueryParam = "meax_role";

    public static IServiceCollection AddHubAuth(
        this IServiceCollection services,
        IConfiguration config)
    {
        var secret = config["Jwt:Secret"];

        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException(
                "Falta la config Jwt:Secret (debe ser idéntica byte a byte a la del hub).");
        }

        // HS256 exige una clave de al menos 32 bytes; con una más corta la
        // validación falla en cada request (IDX10720/IDX10503) y el único
        // síntoma visible es un bucle silencioso hacia /auth/sso.
        if (Encoding.UTF8.GetBytes(secret).Length < 32)
        {
            throw new InvalidOperationException(
                "Jwt:Secret debe tener al menos 32 bytes para HS256.");
        }

        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = SchemeName;
            })
            // Selector: si la petición trae un token del hub se valida el JWT
            // (claims frescos, incluye SystemRole); si no, se usa la cookie local.
            .AddPolicyScheme(SchemeName, "Meax Hub JWT o cookie local", options =>
            {
                options.ForwardDefaultSelector = context =>
                    context.Request.Cookies.ContainsKey(HubJwtCookie)
                    || context.Request.Query.ContainsKey(TokenQueryParam)
                        ? JwtBearerDefaults.AuthenticationScheme
                        : CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = config["Jwt:Issuer"] ?? "meaxHub",
                    ValidateAudience = true,
                    ValidAudience = config["Jwt:Audience"] ?? "meax-services",
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1),
                    NameClaimType = MeaxClaims.DisplayName,
                    RoleClaimType = MeaxClaims.SystemRole
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // 1) Cookie .MEAX.JWT (mismo host que el hub)
                        // 2) Query ?meax_token= (flujo SSO deep-link)
                        var token = context.Request.Cookies[HubJwtCookie];

                        if (string.IsNullOrWhiteSpace(token))
                        {
                            token = context.Request.Query[TokenQueryParam];
                        }

                        if (!string.IsNullOrWhiteSpace(token))
                        {
                            context.Token = token;
                        }

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        if (context.Principal?.Identity is not ClaimsIdentity identity)
                        {
                            return Task.CompletedTask;
                        }

                        var pcLoginId =
                            identity.FindFirst(MeaxClaims.PcLoginId)?.Value ??
                            identity.FindFirst(MeaxClaims.Sub)?.Value;

                        if (string.IsNullOrWhiteSpace(pcLoginId))
                        {
                            context.Fail("El token del hub no contiene PcLoginId/sub.");
                            return Task.CompletedTask;
                        }

                        if (identity.FindFirst(MeaxClaims.PcLoginId) is null)
                        {
                            identity.AddClaim(new Claim(MeaxClaims.PcLoginId, pcLoginId));
                        }

                        if (identity.FindFirst(ClaimTypes.NameIdentifier) is null)
                        {
                            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, pcLoginId));
                        }

                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        // No responder 401: redirigir al SSO del hub.
                        context.HandleResponse();
                        context.Response.Redirect(BuildSsoUrl(context.HttpContext, config));
                        return Task.CompletedTask;
                    }
                };
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "ShippingRequestSystem.Auth";
                options.Cookie.Path = "/";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;

                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.Redirect(BuildSsoUrl(context.HttpContext, config));
                    return Task.CompletedTask;
                };

                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.Redirect(BuildSsoUrl(context.HttpContext, config));
                    return Task.CompletedTask;
                };
            });

        return services;
    }

    /// <summary>
    /// Tras autenticar un token recibido por query string, emite la cookie de
    /// sesión local con los mismos claims y redirige a la misma URL sin
    /// meax_token/meax_role (URL limpia y sesión que sobrevive al token de 5 min).
    /// </summary>
    public static IApplicationBuilder UseHubSsoTokenBridge(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            if (HttpMethods.IsGet(context.Request.Method)
                && context.Request.Query.ContainsKey(TokenQueryParam)
                && context.User.Identity?.IsAuthenticated == true)
            {
                var identity = new ClaimsIdentity(
                    context.User.Claims.Select(c => new Claim(c.Type, c.Value, c.ValueType)),
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    nameType: MeaxClaims.DisplayName,
                    roleType: MeaxClaims.SystemRole);

                await context.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        AllowRefresh = true
                    });

                context.Response.Redirect(BuildRutaSinToken(context.Request));
                return;
            }

            await next();
        });
    }

    private static string BuildSsoUrl(HttpContext http, IConfiguration config)
    {
        var baseUrl = (config["Hub:BaseUrl"] ?? string.Empty).TrimEnd('/');
        var systemCode = config["Hub:SystemCode"] ?? string.Empty;

        // returnUrl absoluto incluyendo PathBase (la app puede correr como
        // sub-aplicación de IIS) y query, sin tokens previos del hub.
        var returnUrl = BuildAbsolutaSinToken(http.Request);

        return $"{baseUrl}/auth/sso" +
               $"?system={Uri.EscapeDataString(systemCode)}" +
               $"&returnUrl={Uri.EscapeDataString(returnUrl)}";
    }

    private static string BuildAbsolutaSinToken(HttpRequest request)
        => UriHelper.BuildAbsolute(
            request.Scheme,
            request.Host,
            request.PathBase,
            request.Path,
            QueryStringSinToken(request));

    private static string BuildRutaSinToken(HttpRequest request)
    {
        var ruta = request.PathBase + request.Path + QueryStringSinToken(request);
        // request.Path nunca es realmente vacío para un request válido (es al
        // menos "/"), pero por si acaso el fallback respeta el PathBase en vez
        // de una "/" fija.
        return string.IsNullOrEmpty(ruta) ? request.PathBase + "/" : ruta;
    }

    private static QueryString QueryStringSinToken(HttpRequest request)
    {
        var query = QueryHelpers.ParseQuery(request.QueryString.Value ?? string.Empty);
        query.Remove(TokenQueryParam);
        query.Remove(RoleQueryParam);

        var builder = new QueryBuilder();

        foreach (var kv in query)
        {
            foreach (var valor in kv.Value)
            {
                builder.Add(kv.Key, valor ?? string.Empty);
            }
        }

        return builder.ToQueryString();
    }
}

using Microsoft.EntityFrameworkCore;
using ShippingRequestSystem.Auth;
using ShippingRequestSystem.Components;
using ShippingRequestSystem.Data;
using ShippingRequestSystem.Middleware;
using ShippingRequestSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Base de datos ─────────────────────────────────────────────────────────────
// Factory para Blazor Server (evita concurrencia de DbContext)
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure()));

// AddDbContext para migraciones automáticas y AuthController
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure()));

// ── Autenticación ─────────────────────────────────────────────────────────────
// Doble candado: el login de desarrollo solo existe si el entorno es
// Development Y Auth:DevLogin es true (en IIS el entorno es Production, así
// que el modo dev es imposible ahí aunque el flag se copie por error).
var devLoginActivo = builder.Environment.IsDevelopment()
    && builder.Configuration.GetValue<bool>("Auth:DevLogin");

builder.Services.AddSingleton(new AuthMode(devLoginActivo));

if (devLoginActivo)
{
    builder.Services.AddDevAuth();
}
else
{
    builder.Services.AddHubAuth(builder.Configuration);
}

// La autorización global NO se hace con una política de fallback sobre los
// endpoints (RequireAuthorization() en MapRazorComponents también protegería
// _framework/blazor.web.js y colgaría la app antes de poder autenticar) — se
// hace con RequireAuthMiddleware más abajo, que respeta [AllowAnonymous].
builder.Services.AddAuthorization();

// ── MVC para AuthController ───────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Blazor Server ─────────────────────────────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ── Servicios de aplicación ───────────────────────────────────────────────────
builder.Services.AddScoped<IApprovalService, ApprovalService>();
builder.Services.AddScoped<IShippingRequestService, ShippingRequestService>();
builder.Services.AddScoped<IQuoteService, QuoteService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IPartCatalogService, PartCatalogService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<FolioService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// ── Middleware ────────────────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();

if (!devLoginActivo)
{
    // Convierte el token SSO recibido por query (?meax_token=) en cookie de
    // sesión local y limpia la URL.
    app.UseHubSsoTokenBridge();
}

app.UseAuthorization();
app.UseMiddleware<RequireAuthMiddleware>();
app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

// ── Auto-migración ────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        var log = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        log.LogError(ex, "Error al aplicar migraciones");
    }
}

app.Run();
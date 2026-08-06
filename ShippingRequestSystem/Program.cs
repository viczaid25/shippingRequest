using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
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
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/auth/logout";
        options.AccessDeniedPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = "ShipRequest.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

builder.Services.AddAuthorization();

// ── MVC para AuthController ───────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Blazor Server ─────────────────────────────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ── Servicios de aplicación ───────────────────────────────────────────────────
builder.Services.AddScoped<IAdService, AdService>();
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
app.UseAuthorization();
app.UseMiddleware<AuthMiddleware>();
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
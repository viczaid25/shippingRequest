using Microsoft.EntityFrameworkCore;
using ShippingRequestSystem.Models;

namespace ShippingRequestSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ShippingRequest> ShippingRequests => Set<ShippingRequest>();
    public DbSet<ShippingPackage> ShippingPackages => Set<ShippingPackage>();
    public DbSet<ShippingItem> ShippingItems => Set<ShippingItem>();
    public DbSet<PremiumFreightInfo> PremiumFreightInfos => Set<PremiumFreightInfo>();
    public DbSet<ApprovalStep> ApprovalSteps => Set<ApprovalStep>();
    public DbSet<ShippingQuote> ShippingQuotes => Set<ShippingQuote>();
    public DbSet<ApprovalRolePermission> ApprovalRolePermissions => Set<ApprovalRolePermission>();
    public DbSet<SystemAdmin> SystemAdmins => Set<SystemAdmin>();
    public DbSet<FolioCounter> FolioCounters => Set<FolioCounter>();
    public DbSet<PartCatalog> PartCatalogs => Set<PartCatalog>();

    // Tabla existente — solo lectura desde EF
    public DbSet<MeaxUser> MeaxUsers => Set<MeaxUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ShippingRequest>(e =>
        {
            e.HasMany(r => r.Packages)
             .WithOne(p => p.ShippingRequest)
             .HasForeignKey(p => p.ShippingRequestId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(r => r.PremiumFreightInfo)
             .WithOne(p => p.ShippingRequest)
             .HasForeignKey<PremiumFreightInfo>(p => p.ShippingRequestId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(r => r.ApprovalSteps)
             .WithOne(a => a.ShippingRequest)
             .HasForeignKey(a => a.ShippingRequestId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(r => r.Quotes)
             .WithOne(q => q.ShippingRequest)
             .HasForeignKey(q => q.ShippingRequestId)
             .OnDelete(DeleteBehavior.Cascade);

            e.Ignore(r => r.IsPremiumFreight);
            e.Property(r => r.IgiPercentage).HasPrecision(18, 4);
            e.Property(r => r.IgiPercentageStaff).HasPrecision(18, 4);
            e.Property(r => r.ApproximateCost).HasPrecision(18, 2);
        });

        modelBuilder.Entity<ShippingPackage>(e =>
        {
            e.HasOne(p => p.ParentPallet)
             .WithMany(p => p.ChildBoxes)
             .HasForeignKey(p => p.ParentPalletId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasMany(p => p.Items)
             .WithOne(i => i.ShippingPackage)
             .HasForeignKey(i => i.ShippingPackageId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // meax_all_user es tabla existente — sin migraciones sobre ella
        modelBuilder.Entity<MeaxUser>(e =>
        {
            e.ToTable("meax_all_user", tb => tb.ExcludeFromMigrations());
            e.HasKey(u => u.Id);
            e.Property(u => u.Id).HasColumnName("id");
            e.Property(u => u.PcLoginId).HasColumnName("pc_login_id").HasMaxLength(50);
            e.Property(u => u.Username).HasColumnName("username").HasMaxLength(100);
            e.Property(u => u.Email).HasColumnName("email").HasMaxLength(100);
            e.Property(u => u.Department).HasColumnName("department").HasMaxLength(50);
            e.Property(u => u.Position).HasColumnName("position").HasMaxLength(50);
            e.Property(u => u.Dep2).HasColumnName("dep_2").HasMaxLength(50);
            // auth es tinyint — se ignora completamente
            e.Ignore("auth");
        });

        // Índice único: un usuario no puede tener el mismo rol dos veces
        modelBuilder.Entity<ApprovalRolePermission>(e =>
        {
            e.HasIndex(p => new { p.RoleName, p.MeaxUserId }).IsUnique();
        });

        modelBuilder.Entity<SystemAdmin>(e =>
        {
            e.HasIndex(a => a.MeaxUserId).IsUnique();
        });

        modelBuilder.Entity<PartCatalog>(e =>
        {
            e.HasIndex(p => p.PartNumber).IsUnique();
        });
    }
}
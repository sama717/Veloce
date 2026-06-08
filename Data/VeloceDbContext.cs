using Microsoft.EntityFrameworkCore;
using Veloco.Enums;
using Veloco.Models;

namespace Veloco.Data;

public class VeloceDbContext(DbContextOptions<VeloceDbContext> options) : DbContext(options)
{
    public DbSet<AssetOwnership> AssetOwnerships { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<CarImage> CarImages { get; set; }
    public DbSet<ClientProfile> ClientProfiles { get; set; }
    public DbSet<Dealership> Dealerships { get; set; }
    public DbSet<EmployeeProfile> EmployeeProfiles { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<User> Users { get; set; }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach(var entry in ChangeTracker.Entries<User>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.Status = UserStatus.Deleted;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        #region User

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique().HasFilter("\"Status\" = 0");
            entity.HasIndex(u => u.Username).IsUnique().HasFilter("\"Status\" = 0");

            entity.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(u => u.MiddleName).HasMaxLength(50);
            entity.Property(u => u.LastName).IsRequired().HasMaxLength(50);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
            entity.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.ProfilePicture).HasMaxLength(500);
            entity.Property(u => u.Role).HasConversion<string>();
        });

        modelBuilder.Entity<ClientProfile>(entity =>
        {
            entity.HasOne(cp => cp.User)
                .WithOne(cp => cp.ClientProfile)
                .HasForeignKey<ClientProfile>(cp => cp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(cp => cp.Mode).HasConversion<string>();
        });

        modelBuilder.Entity<EmployeeProfile>(entity =>
        {
            entity.HasOne(ep => ep.User)
                .WithOne(ep => ep.EmployeeProfile)
                .HasForeignKey<EmployeeProfile>(ep => ep.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ep => ep.Dealership)
                .WithMany(ep => ep.Employees)
                .HasForeignKey(ep => ep.DealershipId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(ep => ep.Position).HasConversion<string>();
        });

        #endregion

        #region Car

        modelBuilder.Entity<Car>(entity =>
        {
            entity.Property(c => c.Brand).IsRequired().HasMaxLength(50);
            entity.Property(c => c.Model).IsRequired().HasMaxLength(50);
            entity.Property(c => c.Color).IsRequired().HasMaxLength(30);
            entity.Property(c => c.Description).HasMaxLength(1000);
            entity.Property(c => c.Price).HasColumnType("numeric(18,2)");
            entity.Property(c => c.PricePerDay).HasColumnType("numeric(18,2)");
            entity.Property(c => c.Type).HasConversion<string>();
            entity.Property(c => c.Status).HasConversion<string>();
            entity.Property(c => c.Condition).HasConversion<string>();

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Car_Price_Positive", "\"Price\" >= 0");
                t.HasCheckConstraint("CK_Car_PricePerDay_Positive", "\"PricePerDay\" >= 0");

                t.HasCheckConstraint(
                    "CK_Car_Pricing_Match_ListingType",
                    "(\"Type\" = 'Sale' AND \"Price\" IS NOT NULL AND \"PricePerDay\" IS NULL) OR " +
                    "(\"Type\" = 'Rent' AND \"PricePerDay\" IS NOT NULL AND \"Price\" IS NULL)"
                );
            });
        });
        
        modelBuilder.Entity<CarImage>(entity =>
        {
            entity.Property(ci => ci.ImageUrl).IsRequired().HasMaxLength(500);

            entity.HasOne(ci => ci.Car)
                .WithMany(c => c.Images)
                .HasForeignKey(ci => ci.CarId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        #endregion

        #region Booking

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.Property(b => b.VerificationDocument).IsRequired().HasMaxLength(500);
            entity.Property(b => b.TotalPrice).IsRequired().HasColumnType("numeric(18,2)");
            entity.Property(b => b.Status).HasConversion<string>();

            entity.HasOne(b => b.Car)
                .WithMany(b => b.Bookings)
                .HasForeignKey(b => b.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(b => b.User)
                .WithMany(b => b.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Booking_Dates_Valid",
                "\"EndDate\" >= \"StartDate\""
            ));
        });

        #endregion

        #region Payment

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.Property(p => p.StripePaymentId).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Amount).IsRequired().HasColumnType("numeric(18,2)");
            entity.Property(p => p.Tax).IsRequired().HasColumnType("numeric(18,2)");
            entity.Property(p => p.TotalAmount).IsRequired().HasColumnType("numeric(18,2)");
            entity.Property(p => p.DealershipCut).HasColumnType("numeric(18,2)");
            entity.Property(p => p.OwnerPayout).HasColumnType("numeric(18,2)");
            entity.Property(p => p.Status).HasConversion<string>();

            entity.HasOne(p => p.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Payment_Total_Matches_Sum",
                "\"TotalAmount\" = (\"Amount\" + \"Tax\")"
            ));
        });

        #endregion

        #region Dealership

        modelBuilder.Entity<Dealership>(entity =>
        {
            entity.Property(d => d.Name).IsRequired().HasMaxLength(50);
            entity.Property(d => d.Address).IsRequired().HasMaxLength(200);
            entity.Property(d => d.City).IsRequired().HasMaxLength(100);
            entity.Property(d => d.State).IsRequired().HasMaxLength(100);
            entity.Property(d => d.Country).IsRequired().HasMaxLength(100);
        });

        #endregion

        #region Asset Ownership

        modelBuilder.Entity<AssetOwnership>(entity =>
        {
            entity.HasOne(ao => ao.Car)
                .WithOne(c => c.AssetOwnership)
                .HasForeignKey<AssetOwnership>(ao => ao.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ao => ao.User)
                .WithMany(u => u.AssetOwnerships)
                .HasForeignKey(ao => ao.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(ao => ao.Dealership)
                .WithMany(d => d.AssetOwnerships)
                .HasForeignKey(ao => ao.DealershipId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_AssetOwnership_ExclusiveOwner",
                "(\"UserId\" IS NOT NULL AND \"DealershipId\" IS NULL) OR (\"UserId\" IS NULL AND \"DealershipId\" IS NOT NULL)"
            ));
        });

        #endregion
    }
}
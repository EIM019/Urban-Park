using CarparkManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarparkManagementSystem.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Site> Sites => Set<Site>();

    public DbSet<ParkingSpace> ParkingSpaces => Set<ParkingSpace>();

    public DbSet<Booking> Bookings => Set<Booking>();

    public DbSet<LayoutPosition> LayoutPositions => Set<LayoutPosition>();

    public DbSet<SpaceLending> SpaceLendings => Set<SpaceLending>();

    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Site>()
            .HasIndex(site => site.Code)
            .IsUnique();

        builder.Entity<ParkingSpace>()
            .Property(space => space.Type)
            .HasConversion<string>();

        builder.Entity<ParkingSpace>()
            .HasIndex(space => new { space.SiteId, space.SpaceNumber })
            .IsUnique();

        builder.Entity<ParkingSpace>()
            .HasOne(space => space.LayoutPosition)
            .WithOne(position => position.ParkingSpace)
            .HasForeignKey<LayoutPosition>(position => position.ParkingSpaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Booking>()
            .Property(booking => booking.Status)
            .HasConversion<string>();

        builder.Entity<Booking>()
            .Property(booking => booking.PartyType)
            .HasConversion<string>();

        builder.Entity<Booking>()
            .HasOne(booking => booking.Site)
            .WithMany()
            .HasForeignKey(booking => booking.SiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Booking>()
            .HasOne(booking => booking.ParkingSpace)
            .WithMany(space => space.Bookings)
            .HasForeignKey(booking => booking.ParkingSpaceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Notification>()
            .HasOne(n => n.RelatedBooking)
            .WithMany()
            .HasForeignKey(n => n.RelatedBookingId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Booking>()
            .HasOne<Booking>()
            .WithMany()
            .HasForeignKey(b => b.OverriddenBookingId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

using Microsoft.EntityFrameworkCore;
using BookingSystem.Web.Models;

namespace BookingSystem.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Resident> Residents { get; set; } = null!;
    public DbSet<Admin> Admins { get; set; } = null!;
    public DbSet<Facility> Facilities { get; set; } = null!;
    public DbSet<TimeSlot> TimeSlots { get; set; } = null!;
    public DbSet<Booking> Bookings { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;
    public DbSet<EmailAuditLog> EmailAuditLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Residents configuration
        modelBuilder.Entity<Resident>()
            .HasIndex(r => r.Email)
            .IsUnique();
        modelBuilder.Entity<Resident>()
            .HasIndex(r => r.ApartmentNumber)
            .IsUnique();
        modelBuilder.Entity<Resident>()
            .HasIndex(r => r.Phone)
            .IsUnique();

        // Admins configuration
        modelBuilder.Entity<Admin>()
            .HasIndex(a => a.Username)
            .IsUnique();
        modelBuilder.Entity<Admin>()
            .HasIndex(a => a.Email)
            .IsUnique();

        // TimeSlots configuration
        modelBuilder.Entity<TimeSlot>()
            .HasIndex(t => new { t.FacilityId, t.Date })
            .IsUnique();
        modelBuilder.Entity<TimeSlot>()
            .HasOne(t => t.Facility)
            .WithMany(f => f.TimeSlots)
            .HasForeignKey(t => t.FacilityId)
            .OnDelete(DeleteBehavior.Cascade);

        // Bookings configuration
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Resident)
            .WithMany(r => r.Bookings)
            .HasForeignKey(b => b.ResidentId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.TimeSlot)
            .WithOne(t => t.Booking)
            .HasForeignKey<Booking>(b => b.TimeSlotId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Facility)
            .WithMany(f => f.Bookings)
            .HasForeignKey(b => b.FacilityId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Booking>()
            .HasIndex(b => b.TimeSlotId)
            .IsUnique();
    }
}

using BaliseManager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BaliseManager.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Beacon> Beacons => Set<Beacon>();

    public DbSet<BeaconEvent> BeaconEvents => Set<BeaconEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Beacon>(entity =>
        {
            entity.ToTable("Beacons", table =>
                table.HasCheckConstraint(
                    "CK_Beacons_BatteryLevel",
                    "\"BatteryLevel\" IS NULL OR \"BatteryLevel\" BETWEEN 0 AND 100"
                ));

            entity.HasKey(b => b.Id);

            entity.Property(b => b.Code)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(b => b.Code)
                .IsUnique();

            entity.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(120);

            entity.Property(b => b.Status)
                .HasConversion<string>()
                .HasMaxLength(30);

            entity.Property(b => b.CreatedAtUtc)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<BeaconEvent>(entity =>
        {
            entity.ToTable("BeaconEvents", table =>
                table.HasCheckConstraint(
                    "CK_BeaconEvents_BatteryLevel",
                    "\"BatteryLevel\" IS NULL OR \"BatteryLevel\" BETWEEN 0 AND 100"
                ));

            entity.HasKey(e => e.Id);

            entity.Property(e => e.EventType)
                .HasConversion<string>()
                .HasMaxLength(40);

            entity.Property(e => e.Message)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.OccurredAtUtc)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => new
            {
                e.BeaconId,
                e.OccurredAtUtc
            });

            entity.HasOne(e => e.Beacon)
                .WithMany(b => b.Events)
                .HasForeignKey(e => e.BeaconId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
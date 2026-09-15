using Microsoft.EntityFrameworkCore;
using RoadSense.Domain.Entities;

namespace RoadSense.Infrastructure.Data;

public class RoadSenseDbContext : DbContext
{
    public RoadSenseDbContext(
        DbContextOptions<RoadSenseDbContext> options)
        : base(options)
    {
    }

    public DbSet<Hazard> Hazards => Set<Hazard>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Hazard>(entity =>
        {
            entity.HasKey(h => h.Id);

            entity.Property(h => h.Type)
                .IsRequired();

            entity.Property(h => h.Latitude)
                .IsRequired();

            entity.Property(h => h.Longitude)
                .IsRequired();

            entity.Property(h => h.Location)
                .HasColumnType("geography (point, 4326)")
                .IsRequired();

            entity.Property(h => h.Severity)
                .IsRequired();

            entity.Property(h => h.ReportedAtUtc)
                .IsRequired();

            entity.HasIndex(h => h.Location)
                .HasMethod("gist");
        });
    }
}
using Microsoft.EntityFrameworkCore;
using ParcelApi.Models;

namespace ParcelApi.Data;

public class ParcelDbContext : DbContext
{
    public ParcelDbContext(DbContextOptions<ParcelDbContext> options) : base(options)
    {
    }

    public DbSet<Parcel> Parcels => Set<Parcel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Parcel>()
            .Property(p => p.Location)
            .HasComputedColumnSql(@"ST_SetSRID(ST_MakePoint(""Longitude"", ""Latitude""), 4326)", stored: true);

        modelBuilder.Entity<Parcel>()
            .HasIndex(p => p.Location)
            .HasMethod("GIST");
    }
}

using Microsoft.EntityFrameworkCore;
using ParcelApi.Models;

namespace ParcelApi.Data;

public class ParcelDbContext : DbContext
{
    public ParcelDbContext(DbContextOptions<ParcelDbContext> options) : base(options)
    {
    }

    public DbSet<Parcel> Parcels => Set<Parcel>();
}

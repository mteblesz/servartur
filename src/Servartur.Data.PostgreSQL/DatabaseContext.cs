using Microsoft.EntityFrameworkCore;
using Servartur.Data.PostgreSQL.Entities;

namespace Servartur.Data.PostgreSQL;

internal class DatabaseContext : DbContext
{
    public DbSet<RoomEntity> Rooms => Set<RoomEntity>();
    public DbSet<PlayerEntity> Players => Set<PlayerEntity>();

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

using Microsoft.EntityFrameworkCore;

namespace Servartur.Data.PostgreSQL;

internal class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }
}

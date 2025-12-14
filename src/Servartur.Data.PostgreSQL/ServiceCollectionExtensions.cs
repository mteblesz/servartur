using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Servartur.Data.PostgreSQL.Configuration;

namespace Servartur.Data.PostgreSQL;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services)
    {
        return services
            .AddPostgreSql<DatabaseContext>();
    }

    public static IServiceCollection AddPostgreSql<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddOptions<DatabaseOptions>();

        services.AddSingleton(
            sp =>
            {
                var databaseOptions = sp.GetRequiredService<DatabaseOptions>();

                return new NpgsqlConnectionStringBuilder
                {
                    Host = databaseOptions.Host,
                    Port = databaseOptions.Port,
                    Username = databaseOptions.UserName,
                    Password = databaseOptions.Password,
                    Database = databaseOptions.DatabaseName,
                };
            });

        services.AddDbContext<TContext>((sp, options) =>
        {
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            var databaseOptions = sp.GetRequiredService<DatabaseOptions>();
            var connectionStringBuilder = sp.GetRequiredService<NpgsqlConnectionStringBuilder>();

            options.UseNpgsql(
                connectionStringBuilder.ConnectionString,
                builder => builder
                    .EnableRetryOnFailure()
                    .CommandTimeout(databaseOptions.CommandTimeoutSeconds));
            options.UseSnakeCaseNamingConvention();
        });

        return services;
    }
}

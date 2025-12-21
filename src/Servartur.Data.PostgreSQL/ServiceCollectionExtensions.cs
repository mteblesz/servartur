using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using Servartur.Data.PostgreSQL.Configuration;
using Servartur.Data.PostgreSQL.Rooms;
using Servartur.Domain.DbRepositories;

namespace Servartur.Data.PostgreSQL;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services)
    {
        return services
            .AddPostgreSql<DatabaseContext>()
            .AddScoped<IRoomsDbRepository, RoomsDbRepository>();
    }

    public static IServiceCollection AddPostgreSql<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddBindAndValdiateOptions<DatabaseOptions>();

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

    private static IServiceCollection AddBindAndValdiateOptions<TOptions>(this IServiceCollection services)
        where TOptions : class
    {
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<TOptions>>().Value);

        var configSectionPath = typeof(TOptions).Name.Replace("Options", string.Empty, StringComparison.Ordinal);

        services
            .AddOptions<TOptions>(string.Empty)
            .BindConfiguration(configSectionPath)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}

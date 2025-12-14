using Servartur.Data.PostgreSQL;

namespace Servartur.Api;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddDatabaseServices();

        return services;
    }
}

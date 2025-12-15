using Servartur.ApplicatonServices;
using Servartur.Core.Endpoints;
using Servartur.Data.PostgreSQL;

namespace Servartur.Api;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        return services
            .AddEndpointRegistrarServices()
            .AddApplicationServices()
            .AddDatabaseServices();
    }
}

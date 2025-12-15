using Microsoft.Extensions.DependencyInjection;

namespace Servartur.Core.Endpoints;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEndpointRegistrarServices(this IServiceCollection services)
    {
        services.AddSingleton<EndpointsRegistrar>();

        return services;
    }
}

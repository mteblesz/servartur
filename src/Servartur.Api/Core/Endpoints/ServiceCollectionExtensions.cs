namespace Servartur.Api.Core.Endpoints;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEndpointRegistrarServices(this IServiceCollection services)
    {
        services.AddSingleton<EndpointsRegistrar>();

        return services;
    }
}

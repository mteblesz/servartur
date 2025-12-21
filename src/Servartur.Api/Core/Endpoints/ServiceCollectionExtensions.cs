namespace Servartur.Api.Core.Endpoints;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEndpointRegistrarServicesFromAssemblyContaining(this IServiceCollection services, Type type)
    {
        services.AddSingleton<CoreEndpointsRegistrar>();

        var endpointsGroupRegistrarTypes = type.Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IEndpointsGroupRegistrar)) && t.IsClass && !t.IsAbstract);

        foreach (var groupRegistrarType in endpointsGroupRegistrarTypes)
        {
            services.AddSingleton(typeof(IEndpointsGroupRegistrar), groupRegistrarType);
        }

        return services;
    }
}

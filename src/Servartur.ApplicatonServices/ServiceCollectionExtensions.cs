using Microsoft.Extensions.DependencyInjection;
using Servartur.ApplicatonServices.Services;

namespace Servartur.ApplicatonServices;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services
            .AddScoped<RoomService>();
    }
}

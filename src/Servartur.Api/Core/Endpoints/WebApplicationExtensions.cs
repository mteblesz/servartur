namespace Servartur.Api.Core.Endpoints;

internal static class WebApplicationExtensions
{
    public static WebApplication RegisterEndpoints(this WebApplication application)
    {
        var registrar = application.Services.GetRequiredService<CoreEndpointsRegistrar>();

        registrar.RegisterEndpoints(application);

        return application;
    }
}

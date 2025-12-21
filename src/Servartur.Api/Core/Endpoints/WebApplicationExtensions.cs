namespace Servartur.Api.Core.Endpoints;

internal static class WebApplicationExtensions
{
    public static WebApplication UseEndpoints(this WebApplication application)
    {
        var registrar = application.Services.GetRequiredService<EndpointsRegistrar>();

        registrar.RegisterEndpoints(application);

        return application;
    }
}

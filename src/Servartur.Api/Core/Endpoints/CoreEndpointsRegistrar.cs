namespace Servartur.Api.Core.Endpoints;

internal class CoreEndpointsRegistrar(IEnumerable<IEndpointsGroupRegistrar> registrars)
{
    private readonly IEnumerable<IEndpointsGroupRegistrar> _registrars = registrars;

    public void RegisterEndpoints(WebApplication application)
    {
        foreach (var registrar in _registrars)
        {
            var endpointsGroup = application.MapGroup(registrar.Path);

            registrar.RegisterEndpoints(endpointsGroup);
        }
    }
}

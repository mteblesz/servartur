namespace Servartur.Core.Endpoints;

internal interface IEndpointsGroupRegistrar
{
    string Path { get; }

    void RegisterEndpoints(RouteGroupBuilder builder);
}

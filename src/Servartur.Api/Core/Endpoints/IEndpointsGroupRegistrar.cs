namespace Servartur.Api.Core.Endpoints;

internal interface IEndpointsGroupRegistrar
{
    string Path { get; }

    void RegisterEndpoints(RouteGroupBuilder builder);
}

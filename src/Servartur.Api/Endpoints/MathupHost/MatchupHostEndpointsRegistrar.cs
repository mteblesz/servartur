using Microsoft.AspNetCore.Http.HttpResults;
using Servartur.Api.Core.Endpoints;
using Servartur.Api.Endpoints.MathupHost.Models;
using Servartur.ApplicatonServices.Services;

namespace Servartur.Api.Endpoints.MathupHost;

internal class MatchupHostEndpointsRegistrar : IEndpointsGroupRegistrar
{
    public string Path => "/matchup";

    public void RegisterEndpoints(RouteGroupBuilder builder)
    {
        builder
            .MapPost("/{roomId}/start", StartGameAsync)
            .Produces(StatusCodes.Status201Created);

        builder
            .MapDelete("/{roomId}/player/{playerId}", KickPlayerAsync)
            .Produces(StatusCodes.Status201Created);
    }
    // TODO: add required auth for host

    private async Task<Ok> StartGameAsync(
        StartGameRequest request,
        HttpContext context,
        MatchupService matchupService,
        CancellationToken ct)
    {

        await matchupService.StartGameAsync(request.RoomId, ct);

        return TypedResults.Ok();
    }

    private async Task<Ok> KickPlayerAsync(
        KickPlayerRequest request,
        HttpContext context,
        MatchupService matchupService,
        CancellationToken ct)
    {

        await matchupService.RemovePlayerAsync(request.RoomId, request.PlayerId, ct);

        return TypedResults.Ok();
    }
}

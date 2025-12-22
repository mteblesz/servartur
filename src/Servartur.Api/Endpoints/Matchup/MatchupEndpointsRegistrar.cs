
using Microsoft.AspNetCore.Http.HttpResults;
using Servartur.Api.Core.Endpoints;
using Servartur.Api.Endpoints.Matchup.Models;
using Servartur.ApplicatonServices.Services;

namespace Servartur.Api.Endpoints.Matchup;

internal class MatchupEndpointsRegistrar : IEndpointsGroupRegistrar
{
    public string Path => "/matchup";

    public void RegisterEndpoints(RouteGroupBuilder builder)
    {
        builder
            .MapPost("/room", CreateRoomAsync)
            .Produces(StatusCodes.Status201Created);

        builder
            .MapPost("/player", CreatePlayerAsync)
            .Produces(StatusCodes.Status201Created);
    }

    private async Task<Created<CreateRoomResponse>> CreateRoomAsync(
        HttpContext context,
        MatchupService matchupService,
        CancellationToken ct)
    {
        var id = await matchupService.CreateRoomAsync(ct);

        return TypedResults.Created($"{Path}/{id}", new CreateRoomResponse { Id = id });
    }

    private async Task<Created<CreatePlayerResponse>> CreatePlayerAsync(
        CreatePlayerRequest request,
        HttpContext context,
        MatchupService matchupService,
        CancellationToken ct)
    {
        var id = await matchupService.CreatePlayerAsync(request.Name.Trim(), request.RoomId, ct);

        return TypedResults.Created($"{Path}/{id}", new CreatePlayerResponse { Id = id });
    }
}

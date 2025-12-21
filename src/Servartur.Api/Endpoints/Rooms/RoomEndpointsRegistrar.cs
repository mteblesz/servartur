
using Microsoft.AspNetCore.Http.HttpResults;
using Servartur.Api.Core.Endpoints;
using Servartur.Api.Endpoints.Rooms.Models;
using Servartur.ApplicatonServices.Services;

namespace Servartur.Api.Endpoints.Rooms;

internal class RoomEndpointsRegistrar : IEndpointsGroupRegistrar
{
    public string Path => "/room";

    public void RegisterEndpoints(RouteGroupBuilder builder)
    {
        builder
            .MapPost("/", CreateRoomAsync)
            .Produces(StatusCodes.Status201Created);
    }

    private async Task<Created<CreateRoomResponse>> CreateRoomAsync(
        CreateRoomRequest request,
        HttpContext context,
        RoomService roomService,
        CancellationToken ct)
    {
        var id = await roomService.CreateRoomAsync(ct);

        return TypedResults.Created($"{Path}/{id}", new CreateRoomResponse { Id = id });
    }
}

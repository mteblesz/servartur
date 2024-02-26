using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using servartur.Entities;
using servartur.Models;

namespace servartur.RealTimeUpdates;

// Group membership isn't preserved when a connection reconnects.  !!! https://learn.microsoft.com/en-us/aspnet/core/signalr/groups?view=aspnetcore-8.0

using GameHubContext = IHubContext<GameHub, IGameHubClient>;

public interface IGameHubClient
{
    Task ReceiveMessage(string message);
    Task ReceivePlayerList(List<PlayerInfoDto> updatedPlayerList);
    Task ReceiveRemoval();
}
public class GameHub : Hub<IGameHubClient>
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.ReceiveMessage($"{Context.ConnectionId} has joined.");
    }
}

// https://stackoverflow.com/a/74414966/23287406
public static class GameHubExtensions
{
    public static async Task AddToRoomGroup(this GameHubContext context, int roomId, string hubConnectionId)
    {
        var groupName = roomId.ToString();
        await context.Groups.AddToGroupAsync(hubConnectionId, groupName);
        await context.Clients.Group(groupName).ReceiveMessage($"{hubConnectionId} has joined the group {groupName}.");
    }

    public static async Task RemoveFromRoomGroup(this GameHubContext context, int roomId, string hubConnectionId)
    {
        var groupName = roomId.ToString();
        await context.Groups.RemoveFromGroupAsync(hubConnectionId, groupName);
        await context.Clients.Group(groupName).ReceiveMessage($"{hubConnectionId} has left the group {groupName}.");
    }

    public static void RefreshPlayers(this GameHubContext context, int roomId, List<PlayerInfoDto> players)
    {
        var groupName = roomId.ToString();
        context.Clients.Group(groupName).ReceivePlayerList(players);
    }

    public static async Task SendRemovalInfo(this GameHubContext context, string hubConnectionId)
    {
        await context.Clients.Client(hubConnectionId).ReceiveRemoval();
    }
}


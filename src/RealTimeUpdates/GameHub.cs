using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using servartur.Entities;
using servartur.Models;

namespace servartur.RealTimeUpdates;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

// Group membership isn't preserved when a connection reconnects.  !!! https://learn.microsoft.com/en-us/aspnet/core/signalr/groups?view=aspnetcore-8.0

using GameHubContext = IHubContext<GameHub, IGameHubClient>;

public interface IGameHubClient
{
    Task ReceiveMessage(string message);
    Task ReceivePlayerList(List<PlayerInfoDto> updatedPlayerList);
    Task ReceiveRemoval(string playerId);
}
public class GameHub : Hub<IGameHubClient>
{
    //https://learn.microsoft.com/en-us/aspnet/signalr/overview/guide-to-the-api/mapping-users-to-connections#in-memory-storage
    // private readonly static ConnectionMapping _connections =  new ConnectionMapping();
    // maybe map here 
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.ReceiveMessage($"{Context.ConnectionId} has joined.");
    }
    public async Task JoinRoomGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).ReceiveMessage($"{Context.ConnectionId} has joined the group {groupName}.");
    }
}

// https://stackoverflow.com/a/74414966/23287406
public static class GameHubExtensions
{
    public static async Task RefreshPlayers(this GameHubContext context, int roomId, List<PlayerInfoDto> players)
    {
        var groupName = roomId.ToString();
        await context.Clients.Group(groupName).ReceivePlayerList(players);
    }

    public static async Task SendRemovalInfo(this GameHubContext context, int roomId, int playerId)
    {
        var groupName = roomId.ToString();
        await context.Clients.Group(groupName).ReceiveRemoval(playerId.ToString());
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using servartur.Entities;
using servartur.Models;
using System.Collections.Generic;

namespace servartur.RealTimeUpdates;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

public interface IGameHubClient
{
    Task ReceiveMessage(string message);
    Task ReceivePlayerList(List<PlayerInfoDto> updatedPlayers);
    Task ReceiveRemoval(string playerId);
    Task ReceiveStartGame();
}
public class GameHub : Hub<IGameHubClient>
{
    //https://learn.microsoft.com/en-us/aspnet/signalr/overview/guide-to-the-api/mapping-users-to-connections#in-memory-storage
    // private readonly static ConnectionMapping _connections =  new ConnectionMapping();
    // TODO maybe map here 
    //public override async Task OnConnectedAsync()
    //{
    //    //await Clients.Caller.ReceiveMessage($"{Context.ConnectionId} has joined.");
    //}
    public async Task JoinRoomGroup(string groupName)
    {
        // Group membership isn't preserved when a connection reconnects.  !!! https://learn.microsoft.com/en-us/aspnet/core/signalr/groups?view=aspnetcore-8.0
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).ReceiveMessage($"{Context.ConnectionId} has joined the group {groupName}.");
    }
}

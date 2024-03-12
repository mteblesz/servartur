using Microsoft.AspNetCore.SignalR;
using servartur.Models.Outgoing;

namespace servartur.RealTimeUpdates;


public interface IGameHubClient
{
    Task ReceiveMessage(string message);
    Task ReceivePlayerList(List<PlayerInfoDto> updatedPlayers);
    Task ReceiveRemoval(string playerId);
    Task ReceiveStartGame();

    Task ReceiveCurrentSquad(SquadInfoDto updatedCurrentSquad);
    Task ReceiveSquadsSummary(List<QuestInfoShortDto> updatedQuestsSummary);

    Task ReceivePlayerLeft(PlayerInfoDto playerInfoDto);
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

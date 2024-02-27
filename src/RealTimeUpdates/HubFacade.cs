using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.SignalR;
using servartur.Models;
using servartur.Services;

namespace servartur.RealTimeUpdates;

public interface IHubFacade
{
    Task AddToRoomGroup(int roomId, string hubConnectionId);
    Task RefreshPlayers(int roomId, List<PlayerInfoDto> players);
    Task RemoveFromRoomGroup(int roomId, string hubConnectionId);
    Task SendRemovalInfo(int playerId);
}

public class HubFacade : IHubFacade
{
    private readonly IHubContext<GameHub, IGameHubClient> _hubContext;
    private readonly IConnectionMapping _connections;

    public HubFacade(IHubContext<GameHub, IGameHubClient> hubContext, IConnectionMapping connections)
    {
        this._hubContext = hubContext;
        this._connections = connections;
    }

    public async Task AddToRoomGroup(int roomId, string hubConnectionId)
    {
        var groupName = roomId.ToString();
        await _hubContext.Groups.AddToGroupAsync(hubConnectionId, groupName);
        await _hubContext.Clients.Group(groupName).ReceiveMessage($"{hubConnectionId} has joined the group {groupName}.");
    }

    public async Task RemoveFromRoomGroup(int roomId, string hubConnectionId)
    {
        var groupName = roomId.ToString();
        await _hubContext.Groups.RemoveFromGroupAsync(hubConnectionId, groupName);
        await _hubContext.Clients.Group(groupName).ReceiveMessage($"{hubConnectionId} has left the group {groupName}.");
    }

    public async Task RefreshPlayers(int roomId, List<PlayerInfoDto> players)
    {
        var groupName = roomId.ToString();
        await _hubContext.Clients.Group(groupName).ReceivePlayerList(players);
    }

    public async Task SendRemovalInfo(int playerId)
    {
        foreach (var connectionId in _connections.GetConnections(playerId))
        {
            await _hubContext.Clients.Client(connectionId).ReceiveRemoval();
        }
    }
}

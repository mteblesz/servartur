using Microsoft.AspNetCore.SignalR;
using servartur.Entities;
using servartur.Models;

namespace servartur.RealTimeUpdates;

public interface IGameClient
{
    Task ReceiveMessage(string message);
    Task ReceivePlayerList(List<PlayerInfoDto> updatedPlayerList);
}
public class GameHub : Hub<IGameClient>
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.ReceiveMessage($"{Context.ConnectionId} has joined.");
    }
    public async Task SendPlayerListUpdate(List<PlayerInfoDto> updatedPlayerList)
    {
        await Clients.All.ReceivePlayerList(updatedPlayerList);
    }
}

// TODO add groups Clients.Group("SignalR Users").SendAsync(
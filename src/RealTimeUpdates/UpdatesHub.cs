using Microsoft.AspNetCore.SignalR;
using servartur.Entities;

namespace servartur.RealTimeUpdates;

public class UpdatesHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined.");
    }
}

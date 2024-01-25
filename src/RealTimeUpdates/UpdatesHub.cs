using Microsoft.AspNetCore.SignalR;

namespace servartur.RealTimeUpdates;

public class UpdatesHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        try
        {
            await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined.");
            Console.WriteLine($"{Context.ConnectionId} has joined.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnConnectedAsync: {ex.Message}");
        }
    }
}

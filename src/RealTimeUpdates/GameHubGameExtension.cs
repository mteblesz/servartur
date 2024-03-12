using Microsoft.AspNetCore.SignalR;
namespace servartur.RealTimeUpdates;

using global::servartur.Models.Outgoing;
using GameHubContext = IHubContext<GameHub, IGameHubClient>;

// https://stackoverflow.com/a/74414966/23287406
public static class GameHubGameExtensions
{
    public static async Task SendPlayerLeftInfo(this GameHubContext context, int roomId, PlayerInfoDto playerInfoDto)
    {
        var groupName = roomId.ToString();
        await context.Clients.Group(groupName).ReceivePlayerLeft(playerInfoDto);
    }
}

using Microsoft.AspNetCore.SignalR;
using servartur.Models;

namespace servartur.RealTimeUpdates;

using GameHubContext = IHubContext<GameHub, IGameHubClient>;

// https://stackoverflow.com/a/74414966/23287406
public static class GameHubMatchupExtensions
{
    public static async Task RefreshPlayers(this GameHubContext context, int roomId, List<PlayerInfoDto> updatedPlayers)
    {
        var groupName = roomId.ToString();
        await context.Clients.Group(groupName).ReceivePlayerList(updatedPlayers);
    }
    public static async Task SendRemovalInfo(this GameHubContext context, int roomId, int playerId)
    {
        var groupName = roomId.ToString();
        await context.Clients.Group(groupName).ReceiveRemoval(playerId.ToString());
    }
    public static async Task SendStartGame(this GameHubContext context, int roomId)
    {
        var groupName = roomId.ToString();
        await context.Clients.Group(groupName).ReceiveStartGame();
    }
}

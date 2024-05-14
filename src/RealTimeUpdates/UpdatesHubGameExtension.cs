using Microsoft.AspNetCore.SignalR;
namespace servartur.RealTimeUpdates;

using global::servartur.Models.Outgoing;
using UpdatesHubContext = IHubContext<UpdatesHub, IUpdatesHubClient>;

// https://stackoverflow.com/a/74414966/23287406
public static class UpdatesHubGameExtensions
{
    public static async Task SendPlayerLeftInfo(this UpdatesHubContext context, int roomId, PlayerInfoDto playerInfoDto)
    {
        var groupName = roomId.ToString();
        await context.Clients.Group(groupName).ReceivePlayerLeft(playerInfoDto);
    }
    public static async Task RefreshEndGameInfo(this UpdatesHubContext context, int roomId, EndGameInfoDto endGameInfoDto)
    {
        var groupName = roomId.ToString();
        await context.Clients.Group(groupName).ReceiveEndGameInfo(endGameInfoDto);
    }
}

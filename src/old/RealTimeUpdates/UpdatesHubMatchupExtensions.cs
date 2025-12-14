using servartur.Models.Outgoing;

using UpdatesHubContext = Microsoft.AspNetCore.SignalR.IHubContext<servartur.RealTimeUpdates.UpdatesHub, servartur.RealTimeUpdates.IUpdatesHubClient>;

namespace servartur.RealTimeUpdates;
// https://stackoverflow.com/a/74414966/23287406
internal static class UpdatesHubMatchupExtensions
{
#pragma warning disable CA1002 // Do not expose generic lists
    public static async Task RefreshPlayers(this UpdatesHubContext context, int roomId, List<PlayerInfoDto> updatedPlayers)
#pragma warning restore CA1002 // Do not expose generic lists
    {
        var groupName = roomId.ToString();
        await context.Clients.Group(groupName).ReceivePlayerList(updatedPlayers);
    }
    public static async Task SendRemovalInfo(this UpdatesHubContext context, int roomId, int playerId)
    {
        var groupName = roomId.ToString();
        await context.Clients.Group(groupName).ReceiveRemoval(playerId.ToString());
    }
    public static async Task SendStartGame(this UpdatesHubContext context, int roomId)
    {
        var groupName = roomId.ToString();
        await context.Clients.Group(groupName).ReceiveStartGame();
    }
}

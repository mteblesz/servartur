using servartur.Models.Outgoing;
using UpdatesHubContext = Microsoft.AspNetCore.SignalR.IHubContext<servartur.RealTimeUpdates.UpdatesHub, servartur.RealTimeUpdates.IUpdatesHubClient>;

namespace servartur.RealTimeUpdates;
// https://stackoverflow.com/a/74414966/23287406
internal static class UpdatesHubGameExtensions
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

    public static async Task RefreshSquadVotingEndedInfo(this UpdatesHubContext context, int roomId, VotingSquadEndedInfoDto votingSquadEndedInfoDto)
    {
        var groupName = roomId.ToString();
        await context.Clients.Group(groupName).ReceiveVotingSquadEndedInfo(votingSquadEndedInfoDto);
    }
    public static async Task RefreshQuestVotingEndedInfo(this UpdatesHubContext context, int roomId, VotingQuestEndedInfoDto votingQuestEndedInfoDto)
    {
        var groupName = roomId.ToString();
        await context.Clients.Group(groupName).ReceiveVotingQuestEndedInfo(votingQuestEndedInfoDto);
    }
}

using Microsoft.AspNetCore.SignalR;
using servartur.Models.Outgoing;

using UpdatesHubContext = Microsoft.AspNetCore.SignalR.IHubContext<servartur.RealTimeUpdates.UpdatesHub, servartur.RealTimeUpdates.IUpdatesHubClient>;

namespace servartur.RealTimeUpdates;

internal static class UpdatesHubSquadsExtensions
{
    public static async Task RefreshCurrentSquad(this UpdatesHubContext context, int roomId, SquadInfoDto updatedCurrentSquad)
    {
        var groupName = roomId.ToString();
        await context.Clients.Group(groupName).ReceiveCurrentSquad(updatedCurrentSquad);
    }
    public static async Task RefreshQuestsSummary(this UpdatesHubContext context, int roomId, List<QuestInfoShortDto> updatedQuestsSummary)
    {
        var groupName = roomId.ToString();
        await context.Clients.Group(groupName).ReceiveQuestsSummary(updatedQuestsSummary);
    }
}

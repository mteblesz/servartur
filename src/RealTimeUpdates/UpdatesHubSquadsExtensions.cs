using Microsoft.AspNetCore.SignalR;
using servartur.Models.Outgoing;

namespace servartur.RealTimeUpdates;

using UpdatesHubContext = IHubContext<UpdatesHub, IUpdatesHubClient>;

public static class UpdatesHubSquadsExtensions
{
    public static async Task RefreshCurrentSquad(this UpdatesHubContext context, int roomId, SquadInfoDto updatedCurrentSquad)
    {
        var groupName = roomId.ToString();
        await context.Clients.Group(groupName).ReceiveCurrentSquad(updatedCurrentSquad);
    }
    public static async Task RefreshSquadsSummary(this UpdatesHubContext context, int roomId, List<QuestInfoShortDto> updatedQuestsSummary)
    {
        var groupName = roomId.ToString();
        await context.Clients.Group(groupName).ReceiveSquadsSummary(updatedQuestsSummary);
    }
}
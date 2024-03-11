using Microsoft.AspNetCore.SignalR;
using servartur.Models.Outgoing;

namespace servartur.RealTimeUpdates;

using GameHubContext = IHubContext<GameHub, IGameHubClient>;

public static class GameHubSquadsExtensions
{
    public static async Task RefreshCurrentSquad(this GameHubContext context, int roomId, SquadInfoDto updatedCurrentSquad)
    {
        var groupName = roomId.ToString();
        await context.Clients.Group(groupName).ReceiveCurrentSquad(updatedCurrentSquad);
    }
    public static async Task RefreshSquadsSummary(this GameHubContext context, int roomId, List<QuestInfoShortDto> updatedQuestsSummary)
    {
        var groupName = roomId.ToString();
        await context.Clients.Group(groupName).ReceiveSquadsSummary(updatedQuestsSummary);
    }
}
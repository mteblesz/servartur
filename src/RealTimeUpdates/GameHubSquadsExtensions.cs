using Microsoft.AspNetCore.SignalR;
using servartur.Models.Outgoing;

namespace servartur.RealTimeUpdates;

using GameHubContext = IHubContext<GameHub, IGameHubClient>;

public static class GameHubSquadsExtensions
{
    public static async Task RefreshSquads(this GameHubContext context, int roomId, List<SquadInfoDto> updatedSquads)
    {
        var groupName = roomId.ToString();
        await context.Clients.Group(groupName).ReceiveSquadsList(updatedSquads);
    }
}
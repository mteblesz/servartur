using Microsoft.AspNetCore.SignalR;
using servartur.Models.Outgoing;

namespace servartur.RealTimeUpdates;

internal interface IUpdatesHubClient
{
    Task ReceiveMessage(string message);

#pragma warning disable CA1002 // Do not expose generic lists
    Task ReceivePlayerList(List<PlayerInfoDto> updatedPlayers);
#pragma warning restore CA1002 // Do not expose generic lists
    Task ReceiveRemoval(string playerId);
    Task ReceiveStartGame();

    Task ReceiveCurrentSquad(SquadInfoDto updatedCurrentSquad);
#pragma warning disable CA1002 // Do not expose generic lists
    Task ReceiveQuestsSummary(List<QuestInfoShortDto> updatedQuestsSummary);
#pragma warning restore CA1002 // Do not expose generic lists

    Task ReceivePlayerLeft(PlayerInfoDto playerInfoDto);
    Task ReceiveEndGameInfo(EndGameInfoDto updatedEndGameInfo);
    Task ReceiveVotingSquadEndedInfo(VotingSquadEndedInfoDto votingSquadEndedInfoDto);
    Task ReceiveVotingQuestEndedInfo(VotingQuestEndedInfoDto votingQuestEndedInfoDto);
}

internal class UpdatesHub : Hub<IUpdatesHubClient>
{
    //https://learn.microsoft.com/en-us/aspnet/signalr/overview/guide-to-the-api/mapping-users-to-connections#in-memory-storage
    // private readonly static ConnectionMapping _connections =  new ConnectionMapping();
    // TODO maybe map here 
    //public override async Task OnConnectedAsync()
    //{
    //    //await Clients.Caller.ReceiveMessage($"{Context.ConnectionId} has joined.");
    //}
    public async Task JoinRoomGroup(string groupName)
    {
        // Group membership isn't preserved when a connection reconnects.  !!! https://learn.microsoft.com/en-us/aspnet/core/signalr/groups?view=aspnetcore-8.0
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        // uncomment line below for debugging groups
        // await Clients.Group(groupName).ReceiveMessage($"{Context.ConnectionId} has joined the group {groupName}.");
    }
}

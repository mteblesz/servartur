using servartur.Entities;
using servartur.Enums;

namespace servartur.DomainLogic;

internal static class SquadFactory
{
    public static Squad OnGameStart(List<Player> roomPlayers)
    {
        Random random = new Random();
        var playerCount = roomPlayers.Count;
        var leader = roomPlayers[random.Next(playerCount)];

        var questNumber = 1;
        Squad firstSquad = new Squad()
        {
            Leader = leader,
            QuestNumber = questNumber,
            SquadNumber = 1,
            RequiredMembersNumber = GameCountsCalculator.GetSquadRequiredSize(playerCount, questNumber),
            IsDoubleFail = GameCountsCalculator.IsQuestDoubleFail(playerCount, questNumber),
            Status = SquadStatus.SquadChoice,
        };
        return firstSquad;
    }

    public static Squad OnRejection(List<Player> roomPlayers, Squad prevSquad, Player prevLeader)
    {
        var leaderIndex = roomPlayers.IndexOf(prevLeader);
        var nextLeaderIndex = (leaderIndex + 1) % roomPlayers.Count;
        var nextLeader = roomPlayers[nextLeaderIndex];

        Squad nextSquad = new Squad()
        {
            QuestNumber = prevSquad.QuestNumber,
            SquadNumber = prevSquad.SquadNumber + 1,
            RequiredMembersNumber = prevSquad.RequiredMembersNumber,
            IsDoubleFail = prevSquad.IsDoubleFail,
            Status = SquadStatus.SquadChoice,
            Leader = nextLeader,
        };

        return nextSquad;
    }
    public static Squad OnQuestFinished(List<Player> roomPlayers, Squad prevSquad, Player prevLeader)
    {
        var playerCount = roomPlayers.Count;
        var leaderIndex = roomPlayers.IndexOf(prevLeader);
        var nextLeaderIndex = (leaderIndex + 1) % playerCount;
        var nextLeader = roomPlayers[nextLeaderIndex];

        var nextQuestNumber = prevSquad.QuestNumber + 1;
        Squad nextSquad = new Squad()
        {
            QuestNumber = nextQuestNumber,
            SquadNumber = 1,
            RequiredMembersNumber = GameCountsCalculator
                                    .GetSquadRequiredSize(playerCount, nextQuestNumber),
            IsDoubleFail = GameCountsCalculator
                                    .IsQuestDoubleFail(playerCount, nextQuestNumber),
            Status = SquadStatus.SquadChoice,
            Leader = nextLeader,
        };

        return nextSquad;
    }
}

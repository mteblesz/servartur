using servartur.Entities;
using servartur.Enums;

namespace servartur.DomainLogic;

internal static class ResultsCalculator
{
    internal enum SquadVotingResult
    {
        Unfinished, Approved, Rejected,
    }
    internal enum QuestVotingResult
    {
        Unfinished, Failed, Successful,
    }
    internal enum GameResult
    {
        Unfinished, GoodWin, EvilWin,
    }

#pragma warning disable CA1002 // Do not expose generic lists
    public static SquadVotingResult CountSquadVotes(int playerCount, List<SquadVote> votes)
#pragma warning restore CA1002 // Do not expose generic lists
    {
        if (playerCount != votes.Count)
        {
            return SquadVotingResult.Unfinished;
        }

        var positiveVotesCount = votes.Count(v => v.Value);

        if (positiveVotesCount > playerCount / 2)
        {
            return SquadVotingResult.Approved;
        }
        else
        {
            return SquadVotingResult.Rejected;
        }
    }
#pragma warning disable CA1002 // Do not expose generic lists
    public static QuestVotingResult CountQuestVotes(int membersCount, List<QuestVote> votes, bool isDoubleFail)
#pragma warning restore CA1002 // Do not expose generic lists
    {
        if (membersCount != votes.Count)
        {
            return QuestVotingResult.Unfinished;
        }

        var negativeVotesCount = votes.Count(v => !v.Value);

        if (negativeVotesCount < (isDoubleFail ? 2 : 1))
        {
            return QuestVotingResult.Successful;
        }
        else
        {
            return QuestVotingResult.Failed;
        }
    }
#pragma warning disable CA1002 // Do not expose generic lists
    public static GameResult EvaluateGame(List<Squad> roomSquads)
#pragma warning restore CA1002 // Do not expose generic lists
    {
        var successfulQuestsCount = roomSquads
            .Count(s => s.Status == SquadStatus.Successful);
        var failedQuestsCount = roomSquads
            .Count(s => s.Status == SquadStatus.Failed);

        if (successfulQuestsCount > 3)
        {
            return GameResult.GoodWin;
        }

        if (successfulQuestsCount > 3)
        {
            return GameResult.EvilWin;
        }
        else
        {
            return GameResult.Unfinished;
        }
    }
}

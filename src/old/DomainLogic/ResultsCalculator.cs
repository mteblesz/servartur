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

    public static SquadVotingResult CountSquadVotes(int playerCount, List<SquadVote> votes)
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
    public static QuestVotingResult CountQuestVotes(int membersCount, List<QuestVote> votes, bool isDoubleFail)
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
    public static GameResult EvaluateGame(List<Squad> roomSquads)
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

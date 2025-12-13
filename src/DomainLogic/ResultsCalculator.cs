using servartur.Entities;
using servartur.Enums;

namespace servartur.DomainLogic;

public static class ResultsCalculator
{
    public enum SquadVotingResult
    {
        Unfinished, Approved, Rejected,
    }
    public enum QuestVotingResult
    {
        Unfinished, Failed, Successful,
    }
    public enum GameResult
    {
        Unfinished, GoodWin, EvilWin,
    }

    public static SquadVotingResult CountSquadVotes(int playerCount, List<SquadVote> votes)
    {
        if (playerCount != votes.Count)
            return SquadVotingResult.Unfinished;

        int positiveVotesCount = votes.Where(v => v.Value == true).Count();

        if (positiveVotesCount > playerCount / 2)
            return SquadVotingResult.Approved;
        else
            return SquadVotingResult.Rejected;
    }
    public static QuestVotingResult CountQuestVotes(int membersCount, List<QuestVote> votes, bool isDoubleFail)
    {
        if (membersCount != votes.Count)
            return QuestVotingResult.Unfinished;

        int negativeVotesCount = votes.Where(v => v.Value == false).Count();

        if (negativeVotesCount < (isDoubleFail ? 2 : 1))
            return QuestVotingResult.Successful;
        else
            return QuestVotingResult.Failed;

    }
    public static GameResult EvaluateGame(List<Squad> roomSquads)
    {
        int successfulQuestsCount = roomSquads
            .Where(s => s.Status == SquadStatus.Successful).Count();
        int failedQuestsCount = roomSquads
            .Where(s => s.Status == SquadStatus.Failed).Count();

        if (successfulQuestsCount > 3)
            return GameResult.GoodWin;
        if (successfulQuestsCount > 3)
            return GameResult.EvilWin;
        else
            return GameResult.Unfinished;
    }
}

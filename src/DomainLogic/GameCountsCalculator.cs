using servartur.Enums;
using System.Data;
namespace servartur.DomainLogic;
/// <summary>
/// Handles game rules related to bounds of number of players
/// </summary>
public static class GameCountsCalculator
{
    public static int MaxNumberOfPlayers { get => 10; }
    public static int MinNumberOfPlayers { get => 5; }
    public static bool IsPlayerCountValid(int playersCount)
    => playersCount >= MinNumberOfPlayers && playersCount <= MaxNumberOfPlayers;
    public static bool IsQuestNumberValid(int questNumber)
    => questNumber >= 1 && questNumber <= 5;

    public static int GetEvilPlayersCount(int playersCount)
    {
        if (!IsPlayerCountValid(playersCount))
            throw new ArgumentException("Invalid number of players given");

        return (playersCount - 1) / 2;
    }

    public static int GetSquadRequiredSize(int playersCount, int questNumber)
    {
        if (!IsPlayerCountValid(playersCount))
            throw new ArgumentException("Invalid number of players given");
        if (!IsQuestNumberValid(questNumber))
            throw new ArgumentException("Invalid quest number given");

        return _squadRequiredSizes[playersCount - 5, questNumber - 1];
    }
    private static readonly int[,] _squadRequiredSizes = new int[,]
    {
        {2, 3, 2, 3, 3}, // 5 players
        {2, 3, 4, 3, 4}, // 6 players
        {2, 3, 3, 4, 4}, // 7 players
        {3, 4, 4, 5, 5}, // 8 players
        {3, 4, 4, 5, 5}, // 9 players
        {3, 4, 4, 5, 5}, // 10 players
    };
}
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
    public static bool IsPlayerCountValid(int playerCount)
    => playerCount >= MinNumberOfPlayers && playerCount <= MaxNumberOfPlayers;
    public static bool IsQuestNumberValid(int questNumber)
    => questNumber >= 1 && questNumber <= 5;

    public static int GetEvilPlayerCount(int playerCount)
    {
        if (!IsPlayerCountValid(playerCount))
            throw new ArgumentException("Invalid number of players given");

        return (playerCount - 1) / 2;
    }

    public static int GetSquadRequiredSize(int playerCount, int questNumber)
    {
        if (!IsPlayerCountValid(playerCount))
            throw new ArgumentException("Invalid number of players given");
        if (!IsQuestNumberValid(questNumber))
            throw new ArgumentException("Invalid quest number given");

        return _squadRequiredSizes[playerCount - 5, questNumber - 1];
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
    public static bool IsQuestDoubleFail(int playerCount, int questNumber)
    {
        if (!IsPlayerCountValid(playerCount))
            throw new ArgumentException("Invalid number of players given");
        if (!IsQuestNumberValid(questNumber))
            throw new ArgumentException("Invalid quest number given");

        return playerCount >= 7 && questNumber == 4;
    }
    public static int MaxNumberOfPrevRejection { get => 5; }
}
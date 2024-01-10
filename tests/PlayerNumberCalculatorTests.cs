using servartur.DomainLogic;

namespace servartur.Tests;

public class PlayerNumberCalculatorTests
{
    [Theory]
    [InlineData(5, 2)]
    [InlineData(6, 2)]
    [InlineData(7, 3)]
    [InlineData(8, 3)]
    [InlineData(9, 4)]
    [InlineData(10, 4)]
    public void GetEvilPlayersNumber_ValidInput_ReturnsCorrectResult
        (int playersCount, int expected)
    {
        // Arrange
        // Act
        var result = GameCountsCalculator.GetEvilPlayersCount(playersCount);
        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(4)]
    [InlineData(12)]
    [InlineData(120)]
    public void GetEvilPlayersNumber_InvalidPlayerCount_ThrowsArgumentException
        (int invalidPlayerCount)
    {
        // Arrange
        // Act
        Action action = () => GameCountsCalculator.GetEvilPlayersCount(invalidPlayerCount);
        // Assert
        Assert.Throws<ArgumentException>(action);
    }

    [Theory]
    [InlineData(5, 1, 2)]
    [InlineData(5, 3, 2)]
    [InlineData(5, 5, 3)]
    [InlineData(6, 2, 3)]
    [InlineData(6, 4, 3)]
    [InlineData(7, 2, 3)]
    [InlineData(7, 3, 3)]
    [InlineData(7, 4, 4)]
    [InlineData(8, 4, 5)]
    [InlineData(9, 5, 5)]
    [InlineData(10, 3, 4)]
    public void GetSquadRequiredSize_ValidInput_ReturnsCorrectResult(int playersCount, int questNumber, int expected)
    {
        // Arrange
        // Act
        var result = GameCountsCalculator.GetSquadRequiredSize(playersCount, questNumber);
        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, 4)]
    [InlineData(11, 5)]
    [InlineData(6, 0)]
    [InlineData(7, 6)]
    public void GetSquadRequiredSize_InvalidInput_ThrowsArgumentException(int questNumber, int playersCount)
    {
        // Arrange
        // Act
        Action action = () => GameCountsCalculator.GetSquadRequiredSize(questNumber, playersCount);
        // Assert
        Assert.Throws<ArgumentException>(action);
    }

}
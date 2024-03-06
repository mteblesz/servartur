using servartur.Enums;
using servartur.Models;
using servartur.Services;
using servartur.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace servartur.Tests.ControllersTests;
public class MatchupControllerTests
{
    private readonly MatchupController _controller;
    private readonly Mock<IMatchupService> _matchupServiceMock;

    public MatchupControllerTests()
    {
        _matchupServiceMock = new Mock<IMatchupService>();
        _controller = new MatchupController(_matchupServiceMock.Object);
    }

    [Fact]
    public void CreateRoom_Returns_CreatedResult()
    {
        // Arrange
        // Act
        var result = _controller.CreateRoom();
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public void JoinRoom_Returns_CreatedResult()
    {
        // Arrange
        var playerId = 1;
        // Act
        var result = _controller.JoinRoom(playerId);
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public void SetNickname_Returns_NoContentResult()
    {
        // Arrange
        var playerId = 1;
        var playerNicknameSetDto = new PlayerNicknameSetDto()
        {
            PlayerId = playerId,
            Nick = "test_nick",
        };
        // Act
        var result = _controller.SetNicknameAsync(playerNicknameSetDto);
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void RemovePlayer_Returns_NoContentResult()
    {
        // Arrange
        var playerId = 1;
        //_matchupServiceMock.Setup(m => m.RemovePlayer(It.IsAny<int>()));
        // Act
        var result = _controller.RemovePlayer(playerId);
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContentResult>();
        _matchupServiceMock.Verify(ms => ms.RemovePlayer(It.IsAny<int>()), Times.Once);
    }

    [Theory]
    [InlineData(true, true, true)]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    [InlineData(true, false, false)]
    [InlineData(false, false, false)]
    public void StartGame_ValidDto_ReturnsNoContent(bool MnA, bool PnM, bool OnM)
    {
        var validDto = new StartGameDto
        {
            RoomId = 1,
            AreMerlinAndAssassinInGame = MnA,
            ArePercivalAreMorganaInGame = PnM,
            AreOberonAndMordredInGame = OnM,
        };

        // Act
        var result = _controller.StartGame(validDto);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContentResult>();
        _matchupServiceMock.Verify(ms => ms.StartGame(It.IsAny<StartGameDto>()), Times.Once);
    }

    [Theory]
    [InlineData(false, true, true)]
    [InlineData(false, true, false)]
    [InlineData(false, false, true)]
    public void StartGame_InvalidDto_ReturnsBadRequest(bool MnA, bool PnM, bool OnM)
    {
        // Arrange
        var invalidDto = new StartGameDto
        {
            RoomId = 1,
            AreMerlinAndAssassinInGame = MnA,
            ArePercivalAreMorganaInGame = PnM,
            AreOberonAndMordredInGame = OnM
        };

        // Act
        var result = _controller.StartGame(invalidDto);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestObjectResult>();
        _matchupServiceMock.Verify(ms => ms.StartGame(It.IsAny<StartGameDto>()), Times.Never);
    }
}

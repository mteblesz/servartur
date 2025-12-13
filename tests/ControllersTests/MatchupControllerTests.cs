using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using servartur.Controllers;
using servartur.Models.Incoming;
using servartur.RealTimeUpdates;
using servartur.Services;

namespace servartur.Tests.ControllersTests;

internal class MatchupControllerTests
{
    private readonly MatchupController _controller;
    private readonly Mock<IMatchupService> _matchupServiceMock;
    private readonly Mock<IHubContext<UpdatesHub, IUpdatesHubClient>> _hubContextMock;

    public MatchupControllerTests()
    {
        _matchupServiceMock = new Mock<IMatchupService>();
        _hubContextMock = new Mock<IHubContext<UpdatesHub, IUpdatesHubClient>>();
        _controller = new MatchupController(_matchupServiceMock.Object, _hubContextMock.Object);
    }

    [Fact]
    public void CreateRoomReturnsCreatedResult()
    {
        // Arrange
        // Act
        var result = _controller.CreateRoom();
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public void JoinRoomReturnsCreatedResult()
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
    public void SetNicknameReturnsNoContentResult()
    {
        // Arrange
        var playerId = 1;
        var playerNicknameSetDto = new PlayerNicknameSetDto()
        {
            RoomId = -1,
            PlayerId = playerId,
            Nick = "test_nick",
        };
        // Act
        var result = _controller.SetNickname(playerNicknameSetDto);
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void RemovePlayerReturnsNoContentResult()
    {
        // Arrange
        var playerId = 1;
        var roomId = 0;
        //_matchupServiceMock.Setup(m => m.RemovePlayer(It.IsAny<int>()));
        // Act
        var result = _controller.RemovePlayer(playerId, roomId);
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
    public void StartGameValidDtoReturnsNoContent(bool MnA, bool PnM, bool OnM)
    {
        var validDto = new StartGameDto
        {
            RoomId = 1,
            AreMerlinAndAssassinInGame = MnA,
            ArePercivalAndMorganaInGame = PnM,
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
    public void StartGameInvalidDtoReturnsBadRequest(bool MnA, bool PnM, bool OnM)
    {
        // Arrange
        var invalidDto = new StartGameDto
        {
            RoomId = 1,
            AreMerlinAndAssassinInGame = MnA,
            ArePercivalAndMorganaInGame = PnM,
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

using servartur.Enums;
using servartur.Models;
using servartur.Services;
using servartur.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace servartur.Tests;
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
    public void CreatePlayer_Returns_CreatedResult()
    {
        // Arrange
        var createPlayerDto = new CreatePlayerDto();
        var playerId = 1;
        _matchupServiceMock.Setup(m => m.CreatePlayer(It.IsAny<CreatePlayerDto>()))
            .Returns(playerId);
        // Act
        var result = _controller.CreatePlayer(createPlayerDto);
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CreatedResult>();
        _matchupServiceMock.Verify(ms => ms.CreatePlayer(It.IsAny<CreatePlayerDto>()), Times.Once);
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

    [Fact]
    public void GetRoomById_Returns_OkResultWithContent()
    {
        // Arrange
        var roomId = 1;
        var roomDto = new RoomDto()
        {
            RoomId = roomId,
            Status = RoomStatus.Unknown.ToString(),
            IsFull = true,
            Players = [],
        };
        _matchupServiceMock.Setup(m => m.GetRoomById(It.IsAny<int>()))
            .Returns(roomDto);

        // Act
        var result = _controller.GetRoomById(roomId);

        // Assert
        result.Should().NotBeNull();
        _matchupServiceMock.Verify(ms => ms.GetRoomById(It.IsAny<int>()), Times.Once);
        // Dto included in response is not null
        result.Should().BeOfType<ActionResult<RoomDto>>()
          .Which.Result.Should().BeOfType<OkObjectResult>()
          .Which.Value.Should().NotBeNull();
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
            RoomId= 1,
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

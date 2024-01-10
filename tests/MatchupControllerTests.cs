using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using servartur.Entities;
using servartur.Enums;
using servartur.Models;
using servartur.Services;
using Moq.EntityFrameworkCore;
using servartur.Exceptions;
using System.Linq.Expressions;
using servartur.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace servartur.Tests;
public class MatchupControllerTests
{
    private MatchupController _controller;
    private Mock<IMatchupService> _matchupServiceMock;

    public MatchupControllerTests()
    {
        _matchupServiceMock = new Mock<IMatchupService>();
        _controller = new MatchupController(_matchupServiceMock.Object);
    }

    [Fact]
    public void CreateRoom_Returns_CreatedResult()
    {
        // Arrange
        var createRoomDto = new CreateRoomDto();
        var roomId = 1;
        _matchupServiceMock.Setup(m => m.CreateRoom(It.IsAny<CreateRoomDto>()))
            .Returns(roomId);
        // Act
        var result = _controller.CreateRoom(createRoomDto);
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
        // Dto included in response is not null
        result.Should().BeOfType<ActionResult<RoomDto>>()
          .Which.Result.Should().BeOfType<OkObjectResult>()
          .Which.Value.Should().NotBeNull();
    }

    // TODO StartGame tests
}

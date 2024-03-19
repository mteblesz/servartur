using Microsoft.AspNetCore.Mvc;
using servartur.Controllers;
using servartur.Entities;
using servartur.Enums;
using servartur.Models.Outgoing;
using servartur.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace servartur.Tests.ControllersTests;
public class InfoControllerTests
{
    private readonly InfoController _controller;
    private readonly Mock<IInfoService> _infoServiceMock;

    public InfoControllerTests()
    {
        _infoServiceMock = new Mock<IInfoService>();
        _controller = new InfoController(_infoServiceMock.Object);
    }

    [Fact]
    public void GetRoomById_Returns_OkResultWithContent()
    {
        // Arrange
        var roomId = 1;
        var roomDto = new RoomInfoDto()
        {
            RoomId = roomId,
            Status = RoomStatus.Unknown.ToString(),
            CurrentSquadId = 1,
            Players = [],
        };
        _infoServiceMock.Setup(m => m.GetRoomById(It.IsAny<int>()))
            .Returns(roomDto);

        // Act
        var result = _controller.GetRoomById(roomId);

        // Assert
        result.Should().NotBeNull();
        _infoServiceMock.Verify(ms => ms.GetRoomById(It.IsAny<int>()), Times.Once);
        result.Should().BeOfType<ActionResult<RoomInfoDto>>()
          .Which.Result.Should().BeOfType<OkObjectResult>()
          .Which.Value.Should().NotBeNull();
    }

    [Fact]
    public void GetGoodPlayers_Returns_OkResultWithContent()
    {
        // Arrange
        var roomId = 1;
        var goodPlayersDto = new List<PlayerInfoDto>
            {
                new PlayerInfoDto { PlayerId = 1, Nick = "Player1"},
                new PlayerInfoDto { PlayerId = 2, Nick = "Player2"},
            };
        _infoServiceMock.Setup(m => m.GetFilteredPlayers(It.IsAny<int>(), It.IsAny<Predicate<Player>>(), null))
            .Returns(goodPlayersDto);

        // Act
        var result = _controller.GetGoodPlayers(roomId);

        // Assert
        result.Should().NotBeNull();
        _infoServiceMock.Verify(ms => ms.GetFilteredPlayers(roomId, It.IsAny<Predicate<Player>>(), null), Times.Once);

        result.Should().BeOfType<ActionResult<List<PlayerInfoDto>>>()
          .Which.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeAssignableTo<List<PlayerInfoDto>>()
            .Which.Should().BeEquivalentTo(goodPlayersDto);
    }

    [Fact]
    public void GetEvilPlayers_Returns_OkResultWithContent()
    {
        // Arrange
        var roomId = 1;
        var evilPlayersDto = new List<PlayerInfoDto>
            {
                new PlayerInfoDto { PlayerId = 3, Nick = "Player3"},
                new PlayerInfoDto { PlayerId = 4, Nick = "Player4"},
            };
        _infoServiceMock.Setup(m => m.GetFilteredPlayers(It.IsAny<int>(), It.IsAny<Predicate<Player>>(), null))
            .Returns(evilPlayersDto);

        // Act
        var result = _controller.GetEvilPlayers(roomId);

        // Assert
        result.Should().NotBeNull();
        _infoServiceMock.Verify(ms => ms.GetFilteredPlayers(roomId, It.IsAny<Predicate<Player>>(), null), Times.Once);

        result.Should().BeOfType<ActionResult<List<PlayerInfoDto>>>()
          .Which.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeAssignableTo<List<PlayerInfoDto>>()
            .Which.Should().BeEquivalentTo(evilPlayersDto);
    }
}
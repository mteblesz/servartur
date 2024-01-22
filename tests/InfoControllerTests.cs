using Microsoft.AspNetCore.Mvc;
using servartur.Controllers;
using servartur.Enums;
using servartur.Models;
using servartur.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace servartur.Tests;
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
            IsFull = true,
            Players = [],
        };
        _infoServiceMock.Setup(m => m.GetRoomById(It.IsAny<int>()))
            .Returns(roomDto);

        // Act
        var result = _controller.GetRoomById(roomId);

        // Assert
        result.Should().NotBeNull();
        _infoServiceMock.Verify(ms => ms.GetRoomById(It.IsAny<int>()), Times.Once);
        // Dto included in response is not null
        result.Should().BeOfType<ActionResult<RoomInfoDto>>()
          .Which.Result.Should().BeOfType<OkObjectResult>()
          .Which.Value.Should().NotBeNull();
    }
}

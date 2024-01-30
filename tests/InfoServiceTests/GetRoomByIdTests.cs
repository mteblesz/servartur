using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using servartur.Entities;
using servartur.Enums;
using servartur.Models;
using servartur.Services;
using servartur.Exceptions;
using Moq.EntityFrameworkCore;

namespace servartur.Tests.InfoServiceTests;
public class GetRoomByIdTests
{
    private static DbContextOptions<GameDbContext> getDbOptions()
        => new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: "test_db")
                .Options;
    [Fact]
    public void GetRoomById_ValidRoomId_ReturnsRoomInfoDto()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<InfoService>>();
        var mapperMock = new Mock<IMapper>();
        var infoService = new InfoService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        var roomId = 1;
        var roomStatus = RoomStatus.Matchup;

        var room = new Room()
        {
            RoomId = roomId,
            Status = roomStatus,
            Players = []
        };
        var expectedRoomInfoDto = new RoomInfoDto
        {
            RoomId = roomId,
            Status = roomStatus.ToString(),
            CurrentSquadId = null,
            Players = []
        };

        dbContextMock.Setup(db => db.Rooms).ReturnsDbSet(new List<Room>() { room });
        mapperMock.Setup(m => m.Map<RoomInfoDto>(room)).Returns(expectedRoomInfoDto);

        // Act
        var result = infoService.GetRoomById(roomId);

        // Assert
        result.Should().BeEquivalentTo(expectedRoomInfoDto);
    }

    [Fact]
    public void GetRoomById_InvalidRoomId_ThrowsRoomNotFoundException()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<InfoService>>();
        var mapperMock = new Mock<IMapper>();
        var infoService = new InfoService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        var invalidRoomId = 999;
        dbContextMock.Setup(db => db.Rooms).ReturnsDbSet(new List<Room>());

        // Act and Assert
        Action action = () => infoService.GetRoomById(invalidRoomId);
        Assert.Throws<RoomNotFoundException>(action);
    }
}

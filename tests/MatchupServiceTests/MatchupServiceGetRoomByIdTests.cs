using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using servartur.Entities;
using servartur.Enums;
using servartur.Models;
using servartur.Services;
using servartur.Exceptions;
using Moq.EntityFrameworkCore;

namespace servartur.Tests.MatchupServiceTests;

public class MatchupServiceGetRoomByIdTests
{
    private static DbContextOptions<GameDbContext> getDbOptions()
        => new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateRoom_ValidDto")
                .Options;
    [Fact]
    public void GetRoomById_ValidRoomId_ReturnsRoomDto()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        var roomId = 1;
        var roomStatus = RoomStatus.Matchup;

        var room = new Room()
        {
            RoomId = roomId,
            Status = roomStatus,
            Players = []
        };
        var expectedRoomDto = new RoomInfoDto
        {
            RoomId = roomId,
            Status = roomStatus.ToString(),
            IsFull = false,
            Players = []
        };

        dbContextMock.Setup(db => db.Rooms).ReturnsDbSet(new List<Room>() { room });
        mapperMock.Setup(m => m.Map<RoomInfoDto>(room)).Returns(expectedRoomDto);

        // Act
        var result = matchupService.GetRoomById(roomId);

        // Assert
        result.Should().BeEquivalentTo(expectedRoomDto);
    }

    [Fact]
    public void GetRoomById_InvalidRoomId_ThrowsRoomNotFoundException()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        var invalidRoomId = 999;
        dbContextMock.Setup(db => db.Rooms).ReturnsDbSet(new List<Room>());

        // Act and Assert
        Action action = () => matchupService.GetRoomById(invalidRoomId);
        Assert.Throws<RoomNotFoundException>(action);
    }
}

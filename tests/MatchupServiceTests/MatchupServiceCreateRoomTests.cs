using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using servartur.Entities;
using servartur.Enums;
using servartur.Models;
using servartur.Services;
using Moq.EntityFrameworkCore;

namespace servartur.Tests.MatchupServiceTests;
public class MatchupServiceCreateRoomTests
{
    private static DbContextOptions<GameDbContext> getDbOptions()
        => new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateRoom_ValidDto")
                .Options;

    [Fact]
    public void CreateRoom_ValidDto_ReturnsRoomIdAndAddRoomToDB()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        var expectedRoomId = 0; // InMemoryDatabase indexing
        var room = new Room { RoomId = expectedRoomId, Status = RoomStatus.Matchup };

        dbContextMock.SetupGet(x => x.Rooms).ReturnsDbSet(new List<Room>());

        // Act
        var result = matchupService.CreateRoom();

        // Assert
        result.Should().Be(expectedRoomId);

        // Ensure the room was added to the DbContext
        dbContextMock.Verify(db => db.Rooms.Add(It.IsAny<Room>()), Times.Once);
        dbContextMock.Verify(db => db.SaveChanges(), Times.Once);
    }
}

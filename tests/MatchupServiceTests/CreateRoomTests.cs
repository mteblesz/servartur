using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq.EntityFrameworkCore;
using servartur.Entities;
using servartur.Enums;
using servartur.Services;

namespace servartur.Tests.MatchupServiceTests;

#pragma warning disable CA1515 // Consider making public types internal
public class CreateRoomTests
#pragma warning restore CA1515 // Consider making public types internal
{
    private static DbContextOptions<GameDbContext> getDbOptions()
    {
        return new DbContextOptionsBuilder<GameDbContext>()
                    .UseInMemoryDatabase(databaseName: "test_db")
                    .Options;
    }

    [Fact]
    public void CreateRoomValidDtoReturnsRoomIdAndAddRoomToDB()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        var expectedRoomId = 0; // InMemoryDatabase indexing
        var room = new Room { RoomId = expectedRoomId, Status = RoomStatus.Matchup };

        dbContextMock.SetupGet(x => x.Rooms).ReturnsDbSet([]);

        // Act
        var result = matchupService.CreateRoom();

        // Assert
        result.Should().Be(expectedRoomId);

        // Ensure the room was added to the DbContext
        dbContextMock.Verify(db => db.Rooms.Add(It.IsAny<Room>()), Times.Once);
        dbContextMock.Verify(db => db.SaveChanges(), Times.Once);
    }
}

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using servartur.Entities;
using servartur.Enums;
using servartur.Exceptions;
using servartur.Services;
using Moq.EntityFrameworkCore;

namespace servartur.Tests.MatchupServiceTests;

public class RemovePlayerTests
{
    private static DbContextOptions<GameDbContext> getDbOptions()
        => new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: "test_db")
                .Options;

    [Fact]
    public void RemovePlayer_ExistingPlayerId_RemovesPlayerFromDB()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        // Create a mock Player in the database
        var existingPlayerId = 1;
        var existingRoomId = 1;
        var existingPlayer = new Player { PlayerId = existingPlayerId, RoomId = existingRoomId };
        var existingPlayers = new List<Player>() { existingPlayer };
        var existingRooms = new List<Room>() {
           new() { RoomId = existingRoomId, Status = RoomStatus.Matchup, Players = existingPlayers }
        };
        dbContextMock.Setup(db => db.Players).ReturnsDbSet(existingPlayers);
        dbContextMock.Setup(db => db.Rooms).ReturnsDbSet(existingRooms);

        // Act
        matchupService.RemovePlayer(existingPlayerId);

        // Assert
        // Ensure the player was removed from the DbContext
        dbContextMock.Verify(db => db.Players.Remove(It.IsAny<Player>()), Times.Once);
        dbContextMock.Verify(db => db.SaveChanges(), Times.Once);
    }

    [Fact]
    public void RemovePlayer_NonExistingPlayerId_ThrowsPlayerNotFoundException()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        // Set up the DbContext to indicate that the specified PlayerId doesn't exist
        var nonExistingPlayerId = 1;
        dbContextMock.Setup(db => db.Players).ReturnsDbSet(new List<Player>());

        // Act and Assert
        Action action = () => matchupService.RemovePlayer(nonExistingPlayerId);
        Assert.Throws<PlayerNotFoundException>(action);

        dbContextMock.Verify(db => db.Players.Remove(It.IsAny<Player>()), Times.Never);
        dbContextMock.Verify(db => db.SaveChanges(), Times.Never);
    }

    [Fact]
    public void RemovePlayer_PlayersRoomIsNotInMatchup_ThrowsRoomNotInMatchupException()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        // Set up the DbContext to indicate that the Player's Room is not in matchup
        var roomStatusOtherThanMatchup = RoomStatus.Playing;
        var existingPlayerId = 1;
        var existingRoomId = 1;
        var existingPlayer = new Player { PlayerId = existingPlayerId, RoomId = existingRoomId };
        var existingPlayers = new List<Player>() { existingPlayer };
        var existingRooms = new List<Room>() {
           new() { RoomId = existingRoomId, Status = roomStatusOtherThanMatchup, Players = existingPlayers }
        };
        dbContextMock.Setup(db => db.Players).ReturnsDbSet(existingPlayers);
        dbContextMock.Setup(db => db.Rooms).ReturnsDbSet(existingRooms);

        // Act and Assert
        Action action = () => matchupService.RemovePlayer(existingPlayerId);
        Assert.Throws<RoomNotInMatchupException>(action);

        dbContextMock.Verify(db => db.Players.Remove(It.IsAny<Player>()), Times.Never);
        dbContextMock.Verify(db => db.SaveChanges(), Times.Never);
    }
}

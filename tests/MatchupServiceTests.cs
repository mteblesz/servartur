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

namespace servartur.Tests;
public class MatchupServiceTests
{
    private static DbContextOptions<GameDbContext> getDbOptions()
        => new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateRoom_ValidDto")
                .Options;

    #region CreateRoom tests
    [Fact]
    public void CreateRoom_ValidDto_ReturnsRoomIdAndAddRoomToDB()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        var createRoomDto = new CreateRoomDto();
        var expectedRoomId = 1;
        var room = new Room { RoomId = expectedRoomId, Status = RoomStatus.Matchup };

        mapperMock.Setup(m => m.Map<Room>(It.IsAny<CreateRoomDto>())).Returns(room);
        dbContextMock.SetupGet(x => x.Rooms).ReturnsDbSet(new List<Room>());

        // Act
        var result = matchupService.CreateRoom(createRoomDto);

        // Assert
        result.Should().Be(expectedRoomId);

        // Ensure the room was added to the DbContext
        dbContextMock.Verify(db => db.Rooms.Add(It.IsAny<Room>()), Times.Once);
        dbContextMock.Verify(db => db.SaveChanges(), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(50)]
    public void CreateMultipleRooms_ValidDto_ReturnsRoomIdAndAddsRoomToDB(int numberOfCreations)
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        var createRoomDto = new CreateRoomDto();
        var expectedRoomIds = Enumerable.Range(1, numberOfCreations).ToList();
        IList<Room> rooms = Enumerable.Range(1, numberOfCreations)
            .Select(i => new Room { RoomId = i, Status = RoomStatus.Matchup }).ToList();

        dbContextMock.SetupGet(x => x.Rooms).ReturnsDbSet(new List<Room>());

        // Setup mapperMock to return subsequent rooms dynamically using RoomProvider.GetNext
        var roomProvider = new RoomProvider(rooms);
        mapperMock.Setup(m => m.Map<Room>(It.IsAny<CreateRoomDto>()))
            .Returns(() => roomProvider.GetNext());

        // Act
        var results = new List<int>();
        for (int i = 0; i < numberOfCreations; i++)
            results.Add(matchupService.CreateRoom(createRoomDto));

        // Assert
        results.Should().BeEquivalentTo(expectedRoomIds);
        dbContextMock.Verify(db => db.Rooms.Add(It.IsAny<Room>()), Times.Exactly(numberOfCreations));
        dbContextMock.Verify(db => db.SaveChanges(), Times.Exactly(numberOfCreations));
    }
    private class RoomProvider(IList<Room> rooms)
    {
        private readonly IList<Room> _rooms = rooms;
        private int i = 0;
        public Room GetNext() => _rooms[i++];
    }
    #endregion

    #region CreatePLayer tests
    [Fact]
    public void CreatePlayer_ValidDto_ReturnsPlayerIdAndAddsPLayerToDB()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        const int roomId = 1;
        var createPlayerDto = new CreatePlayerDto() { RoomId = roomId };
        var expectedPlayerId = 1;
        var player = new Player() { PlayerId = expectedPlayerId, Nick = "test_nick", RoomId = roomId };
        var rooms = new List<Room>() { new() { RoomId = roomId, Status = RoomStatus.Matchup } };

        mapperMock.Setup(m => m.Map<Player>(It.IsAny<CreatePlayerDto>())).Returns(player);
        dbContextMock.SetupGet(x => x.Rooms).ReturnsDbSet(rooms);
        dbContextMock.SetupGet(x => x.Players).ReturnsDbSet(new List<Player>());

        // Act
        var result = matchupService.CreatePlayer(createPlayerDto);

        // Assert
        result.Should().Be(expectedPlayerId);
        dbContextMock.Verify(db => db.Players.Add(It.IsAny<Player>()), Times.Once);
        dbContextMock.Verify(db => db.SaveChanges(), Times.Once);
    }

    [Theory]
    //[InlineData(1)]
    [InlineData(2)]
    //[InlineData(5)]
    //[InlineData(10)]
    public void CreateMultiplePlayer_ValidDto_ReturnsPlayerIdAndAddsPLayerToDB(int numberOfCreations)
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        const int roomId = 1;
        var createPlayerDto = new CreatePlayerDto() { RoomId = roomId };
        var expectedPlayerIds = Enumerable.Range(1, numberOfCreations);
        var players = Enumerable.Range(1, numberOfCreations)
            .Select(i => new Player() { PlayerId = i, Nick = $"test_nick_{i}", RoomId = roomId }).ToList();
        var rooms = new List<Room>() { new() { RoomId = roomId, Status = RoomStatus.Matchup } };

        var playerProvider = new PlayerProvider(players);
        mapperMock.Setup(m => m.Map<Player>(It.IsAny<CreatePlayerDto>()))
            .Returns(() => playerProvider.GetNext());
        dbContextMock.SetupGet(x => x.Rooms).ReturnsDbSet(rooms);
        dbContextMock.SetupGet(x => x.Players).ReturnsDbSet(players);

        // Act
        var results = new List<int>();
        for (int i = 0; i < numberOfCreations; i++)
            results.Add(matchupService.CreatePlayer(createPlayerDto));

        // Assert
        results.Should().BeEquivalentTo(expectedPlayerIds);
        dbContextMock.Verify(db => db.Players.Add(It.IsAny<Player>()), Times.Exactly(numberOfCreations));
        dbContextMock.Verify(db => db.SaveChanges(), Times.Exactly(numberOfCreations));
    }
    private class PlayerProvider(IList<Player> players)
    {
        private readonly IList<Player> _players = players;
        private int i = 0;
        public Player GetNext() => _players[i++];
    }

    [Fact]
    public void CreatePlayer_RoomNotFound_ThrowsRoomNotFoundException()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        var createPlayerDto = new CreatePlayerDto() { RoomId = 100 };

        var rooms = new List<Room>() { new() { RoomId = 1 }, new() { RoomId = 2 } };
        dbContextMock.SetupGet(x => x.Rooms).ReturnsDbSet(rooms);

        // Act and Assert
        Action action = () => matchupService.CreatePlayer(createPlayerDto);
        Assert.Throws<RoomNotFoundException>(action);
    }

    [Theory]
    [InlineData(RoomStatus.Unknown)]
    [InlineData(RoomStatus.Playing)]
    [InlineData(RoomStatus.Assassination)]
    [InlineData(RoomStatus.Result)]
    public void CreatePlayer_RoomNotInMatchup_ThrowsRoomNotInMatchupException(RoomStatus status)
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        const int roomId = 1;
        var createPlayerDto = new CreatePlayerDto() { RoomId = roomId };

        var rooms = new List<Room>() { new() { RoomId = roomId, Status = status } };
        dbContextMock.SetupGet(x => x.Rooms).ReturnsDbSet(rooms);

        // Act and Assert
        Action action = () => matchupService.CreatePlayer(createPlayerDto);
        Assert.Throws<RoomNotInMatchupException>(action);
    }
    [Fact]
    public void CreatePlayer_RoomNotInFull_ThrowsRoomIsFullException()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        const int roomId = 1;
        var createPlayerDto = new CreatePlayerDto() { RoomId = roomId };
        var players = Enumerable.Range(1, 10)
            .Select(i => new Player() { PlayerId = i, Nick = $"test_{i}", RoomId = roomId })
            .ToList();
        var rooms = new List<Room>() { new()
        {
            RoomId = roomId,
            Status = RoomStatus.Matchup,
            Players = players,
        } };

        dbContextMock.SetupGet(x => x.Rooms).ReturnsDbSet(rooms);

        // Act and Assert
        Action action = () => matchupService.CreatePlayer(createPlayerDto);
        Assert.Throws<RoomIsFullException>(action);
    }
    #endregion

    #region RemovePlayer tests
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
        var existingPlayer = new Player { PlayerId = existingPlayerId };
        var existingPlayers = new List<Player>() { existingPlayer };
        dbContextMock.Setup(db => db.Players).ReturnsDbSet(existingPlayers);

        // Act
        matchupService.RemovePlayer(existingPlayerId);

        // Assert
        // Ensure the player was removed from the DbContext
        dbContextMock.Verify(db => db.Players.Remove(existingPlayer), Times.Once);
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
    #endregion

    #region GetRoomById tests
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
            Players = new List<Player>()
        };
        var expectedRoomDto = new RoomDto
        {
            RoomId = roomId,
            Status = roomStatus.ToString(),
            IsFull = false, // Adjust based on your specific criteria for IsFull
            Players = new List<PlayerDto>() // Add players as needed for the test
        };

        dbContextMock.Setup(db => db.Rooms).ReturnsDbSet(new List<Room>() { room });
        mapperMock.Setup(m => m.Map<RoomDto>(room)).Returns(expectedRoomDto);

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
    #endregion

    #region StartGame tests
        // TODO
    #endregion
}

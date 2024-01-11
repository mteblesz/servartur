using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using servartur.Entities;
using servartur.Enums;
using servartur.Exceptions;
using servartur.Models;
using servartur.Services;
using Moq.EntityFrameworkCore;

namespace servartur.Tests.MatchupServiceTests;
public class MatchupServiceCreatePLayerTests
{
    private static DbContextOptions<GameDbContext> getDbOptions()
        => new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateRoom_ValidDto")
                .Options;

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
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
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
}

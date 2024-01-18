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
public class MatchupServiceJoinRoomTests
{
    private static DbContextOptions<GameDbContext> getDbOptions()
        => new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateRoom_ValidDto")
                .Options;

    [Fact]
    public void JoinRoom_ValidDto_ReturnsPlayerIdAndAddsPlayerToDB()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        const int roomId = 1;
        var expectedPlayerId = 0; // InMemoryDatabase indexing
        var room = new Room() { RoomId = roomId, Status = RoomStatus.Matchup };

        dbContextMock.SetupGet(x => x.Rooms).ReturnsDbSet([room]);
        dbContextMock.SetupGet(x => x.Players).ReturnsDbSet(new List<Player>());

        // Act
        var result = matchupService.JoinRoom(roomId);

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
    public void JoinRoomMultiple_ValidDto_ReturnsPlayerIdAndAddsPlayerToDB(int numberOfCreations)
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        const int roomId = 1;
        var expectedPlayerIds = Enumerable.Range(1, numberOfCreations);

        var room = new Room() { RoomId = roomId, Status = RoomStatus.Matchup};
        var rooms = new List<Room> { room };

        var playerIdProvider = new PlayerIdProvider(expectedPlayerIds.ToList());
        dbContextMock.Setup(x => x.Players.Add(It.IsAny<Player>()))
            .Callback<Player>(p =>
            { 
                p.PlayerId = playerIdProvider.GetNext(); // Mock<GameDbContext> won't index propertly by itself
            });

        dbContextMock.SetupGet(x => x.Rooms).ReturnsDbSet(rooms);

        // Act
        var results = new List<int>();
        for (int i = 0; i < numberOfCreations; i++)
            results.Add(matchupService.JoinRoom(roomId));

        // Assert
        results.Should().BeEquivalentTo(expectedPlayerIds);
        dbContextMock.Verify(db => db.Players.Add(It.IsAny<Player>()), Times.Exactly(numberOfCreations));
        dbContextMock.Verify(db => db.SaveChanges(), Times.Exactly(numberOfCreations));
    }
    private class PlayerIdProvider(IList<int> ids)
    {
        private readonly IList<int> _ids = ids;
        private int i = 0;
        public int GetNext() => _ids[i++];
    }

    [Fact]
    public void JoinRoom_RoomNotFound_ThrowsRoomNotFoundException()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        var invalidRoomId = 100;
        var rooms = new List<Room>() { new() { RoomId = 1 }, new() { RoomId = 2 } };
        dbContextMock.SetupGet(x => x.Rooms).ReturnsDbSet(rooms);

        // Act and Assert
        Action action = () => matchupService.JoinRoom(invalidRoomId);
        Assert.Throws<RoomNotFoundException>(action);
    }

    [Theory]
    [InlineData(RoomStatus.Unknown)]
    [InlineData(RoomStatus.Playing)]
    [InlineData(RoomStatus.Assassination)]
    [InlineData(RoomStatus.Result)]
    public void JoinRoom_RoomNotInMatchup_ThrowsRoomNotInMatchupException(RoomStatus status)
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        const int roomId = 1;

        var rooms = new List<Room>() { new() { RoomId = roomId, Status = status } };
        dbContextMock.SetupGet(x => x.Rooms).ReturnsDbSet(rooms);

        // Act and Assert
        Action action = () => matchupService.JoinRoom(roomId);
        Assert.Throws<RoomNotInMatchupException>(action);
    }
    [Fact]
    public void JoinRoom_RoomNotInFull_ThrowsRoomIsFullException()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        const int roomId = 1;
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
        Action action = () => matchupService.JoinRoom(roomId);
        Assert.Throws<RoomIsFullException>(action);
    }

}

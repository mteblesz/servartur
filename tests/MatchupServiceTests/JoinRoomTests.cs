using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq.EntityFrameworkCore;
using servartur.Entities;
using servartur.Enums;
using servartur.Exceptions;
using servartur.Services;

namespace servartur.Tests.MatchupServiceTests;

#pragma warning disable CA1515 // Consider making public types internal
public class JoinRoomTests
#pragma warning restore CA1515 // Consider making public types internal
{
    private static DbContextOptions<GameDbContext> getDbOptions()
    {
        return new DbContextOptionsBuilder<GameDbContext>()
                    .UseInMemoryDatabase(databaseName: "test_db")
                    .Options;
    }

    [Fact]
    public void JoinRoomValidDtoReturnsPlayerIdAndAddsPlayerToDB()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        const int RoomId = 1;
        var expectedPlayerId = 0; // InMemoryDatabase indexing
        var room = new Room() { RoomId = RoomId, Status = RoomStatus.Matchup };

        dbContextMock.SetupGet(x => x.Rooms).ReturnsDbSet([room]);
        dbContextMock.SetupGet(x => x.Players).ReturnsDbSet([]);

        // Act
        var result = matchupService.JoinRoom(RoomId);

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
    public void JoinRoomMultipleValidDtoReturnsPlayerIdAndAddsPlayerToDB(int numberOfCreations)
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        const int RoomId = 1;
        var expectedPlayerIds = Enumerable.Range(1, numberOfCreations);

        var room = new Room() { RoomId = RoomId, Status = RoomStatus.Matchup };
        var rooms = new List<Room> { room };

        var playerIdProvider = new PlayerIdProvider([.. expectedPlayerIds]);
        dbContextMock.Setup(x => x.Players.Add(It.IsAny<Player>()))
            .Callback<Player>(p =>
            {
                p.PlayerId = playerIdProvider.GetNext; // Mock<GameDbContext> won't index propertly by itself
            });

        dbContextMock.SetupGet(x => x.Rooms).ReturnsDbSet(rooms);

        // Act
        var results = new List<int>();
        for (var i = 0; i < numberOfCreations; i++)
        {
            results.Add(matchupService.JoinRoom(RoomId));
        }

        // Assert
        results.Should().BeEquivalentTo(expectedPlayerIds);
        dbContextMock.Verify(db => db.Players.Add(It.IsAny<Player>()), Times.Exactly(numberOfCreations));
        dbContextMock.Verify(db => db.SaveChanges(), Times.Exactly(numberOfCreations));
    }
    private class PlayerIdProvider(IList<int> ids)
    {
        private readonly IList<int> _ids = ids;

        public int GetNext => _ids[field++];
    }

    [Fact]
    public void JoinRoomRoomNotFoundThrowsRoomNotFoundException()
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
        void action() => matchupService.JoinRoom(invalidRoomId);
        Assert.Throws<RoomNotFoundException>(action);
    }

    [Theory]
    [InlineData(RoomStatus.Unknown)]
    [InlineData(RoomStatus.Playing)]
    [InlineData(RoomStatus.Assassination)]
    [InlineData(RoomStatus.ResultEvilWin)]
    public void JoinRoomRoomNotInMatchupThrowsRoomNotInMatchupException(RoomStatus status)
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        const int RoomId = 1;

        var rooms = new List<Room>() { new() { RoomId = RoomId, Status = status } };
        dbContextMock.SetupGet(x => x.Rooms).ReturnsDbSet(rooms);

        // Act and Assert
        void action() => matchupService.JoinRoom(RoomId);
        Assert.Throws<RoomNotInMatchupException>(action);
    }
    [Fact]
    public void JoinRoomRoomNotInFullThrowsRoomIsFullException()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        const int RoomId = 1;
        var players = Enumerable.Range(1, 10)
            .Select(i => new Player() { PlayerId = i, Nick = $"test_{i}", RoomId = RoomId })
            .ToList();
        var rooms = new List<Room>() { new()
        {
            RoomId = RoomId,
            Status = RoomStatus.Matchup,
            Players = players,
        } };

        dbContextMock.SetupGet(x => x.Rooms).ReturnsDbSet(rooms);

        // Act and Assert
        void action() => matchupService.JoinRoom(RoomId);
        Assert.Throws<RoomIsFullException>(action);
    }

}

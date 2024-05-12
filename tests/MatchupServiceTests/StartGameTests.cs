using Microsoft.EntityFrameworkCore;
using servartur.Entities;
using Moq.EntityFrameworkCore;
using AutoMapper;
using Microsoft.Extensions.Logging;
using servartur.Enums;
using servartur.Services;
using servartur.DomainLogic;
using servartur.Exceptions;
using servartur.Models.Incoming;

namespace servartur.Tests.MatchupServiceTests;

public class StartGameTests
{
    private static DbContextOptions<GameDbContext> getDbOptions()
        => new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: "test_db")
                .Options;

    [Theory]
    [InlineData(5, false, false, false)]
    [InlineData(7, true, true, false)]
    [InlineData(10, true, true, true)]
    public void StartGame_ValidInput_StartsGameCreatesFirstSquad(int numberOfPlayers, bool MnA, bool PnM, bool OnM)
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);
        
        // Create room and player list
        const int roomId = 1;
        var startGameDto = new StartGameDto()
        {
            RoomId = roomId,
            AreMerlinAndAssassinInGame = MnA,
            ArePercivalAndMorganaInGame = PnM,
            AreOberonAndMordredInGame = OnM,
        };

        List<Player> players = [];
        for (int i = 1; i < 1 + numberOfPlayers; i++)
            players.Add(new Player() { PlayerId = i, Nick = $"test_nick_{i}", RoomId = roomId });
        var room = new Room() { RoomId = roomId, Status = RoomStatus.Matchup, Players = players };
        List<Room> rooms = [ room ];

        mapperMock.Setup(m => m.Map<GameStartHelper.RoleInfo>(It.IsAny<StartGameDto>()))
            .Returns(new GameStartHelper.RoleInfo(MnA, PnM, OnM));
        dbContextMock.SetupGet(x => x.Rooms).ReturnsDbSet(rooms);
        dbContextMock.SetupGet(x => x.Players).ReturnsDbSet(room.Players);
        dbContextMock.SetupGet(x => x.Squads).ReturnsDbSet(room.Squads);

        // Act
        matchupService.StartGame(startGameDto);

        // Assert
        rooms.First().Status.Should().Be(RoomStatus.Playing);
        foreach (var player in rooms[0].Players)
        {
            player.Role.Should().NotBeNull();
            player.Team.Should().NotBeNull();
        }
        rooms.First().Squads.Should().HaveCount(1);
        rooms.First().CurrentSquad.Should().NotBeNull();
        dbContextMock.Verify(db => db.SaveChanges(), Times.Once);
    }


    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    [InlineData(11)]
    public void StartGame_InvalidPlayerCount_ThrowsPlayerCountInvalidException(int numberOfPlayers)
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        // Create room and player list
        const int roomId = 1;
        var startGameDto = new StartGameDto()
        {
            RoomId = roomId,
            AreMerlinAndAssassinInGame = true,
            ArePercivalAndMorganaInGame = true,
            AreOberonAndMordredInGame = false,
        };

        List<Player> players = [];
        for (int i = 1; i < 1 + numberOfPlayers; i++)
            players.Add(new Player() { PlayerId = i, Nick = $"test_nick_{i}", RoomId = roomId });
        var room = new Room() { RoomId = roomId, Status = RoomStatus.Matchup, Players = players };
        List<Room> rooms = [room];

        dbContextMock.SetupGet(x => x.Rooms).ReturnsDbSet(rooms);
        dbContextMock.SetupGet(x => x.Players).ReturnsDbSet(room.Players);
        dbContextMock.SetupGet(x => x.Squads).ReturnsDbSet(room.Squads);

        // Act
        Action action = () => matchupService.StartGame(startGameDto);

        // Assert
        action.Should().Throw<PlayerCountInvalidException>();
    }

    [Theory]
    [InlineData(RoomStatus.Unknown)]
    [InlineData(RoomStatus.Playing)]
    [InlineData(RoomStatus.Assassination)]
    [InlineData(RoomStatus.ResultGoodWin)]
    public void StartGame_RoomNotInMatchup_ThrowsRoomNotInMatchupException(RoomStatus roomStatus)
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        // Create room and player list
        const int roomId = 1;
        var numberOfPlayers = 7;
        var startGameDto = new StartGameDto()
        {
            RoomId = roomId,
            AreMerlinAndAssassinInGame = true,
            ArePercivalAndMorganaInGame = true,
            AreOberonAndMordredInGame = false,
        };

        List<Player> players = [];
        for (int i = 1; i < 1 + numberOfPlayers; i++)
            players.Add(new Player() { PlayerId = i, Nick = $"test_nick_{i}", RoomId = roomId });
        var room = new Room() { RoomId = roomId, Status = roomStatus, Players = players };
        List<Room> rooms = [room];

        dbContextMock.SetupGet(x => x.Rooms).ReturnsDbSet(rooms);
        dbContextMock.SetupGet(x => x.Players).ReturnsDbSet(room.Players);
        dbContextMock.SetupGet(x => x.Squads).ReturnsDbSet(room.Squads);

        // Act
        Action action = () => matchupService.StartGame(startGameDto);

        // Assert
        action.Should().Throw<RoomNotInMatchupException>();
    }

    [Theory]
    [InlineData(5, true, false, true)]
    [InlineData(6, true, false, true)]
    [InlineData(7, true, true, true)]
    [InlineData(8, true, true, true)]
    public void StartGame_TooManyEvilRoles_ThrowsTooManyEvilRolesException(int numberOfPlayers, bool MnA, bool PnM, bool OnM)
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        // Create room and player list
        const int roomId = 1;
        List<Player> players = [];
        for (int i = 1; i < 1 + numberOfPlayers; i++)
            players.Add(new Player() { PlayerId = i, Nick = $"test_nick_{i}", RoomId = roomId });
        var room = new Room() { RoomId = roomId, Status = RoomStatus.Matchup, Players = players };
        List<Room> rooms = [room];

        var startGameDto = new StartGameDto()
        {
            RoomId = roomId,
            AreMerlinAndAssassinInGame = MnA,
            ArePercivalAndMorganaInGame = PnM,
            AreOberonAndMordredInGame = OnM,
        };

        mapperMock.Setup(m => m.Map<GameStartHelper.RoleInfo>(It.IsAny<StartGameDto>()))
            .Returns(new GameStartHelper.RoleInfo(MnA, PnM, OnM));
        dbContextMock.SetupGet(x => x.Rooms).ReturnsDbSet(rooms);
        dbContextMock.SetupGet(x => x.Players).ReturnsDbSet(room.Players);
        dbContextMock.SetupGet(x => x.Squads).ReturnsDbSet(room.Squads);

        // Act
        Action action = () => matchupService.StartGame(startGameDto);

        // Assert
        action.Should().Throw<TooManyEvilRolesException>();
    }
}

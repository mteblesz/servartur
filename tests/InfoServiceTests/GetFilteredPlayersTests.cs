using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq.EntityFrameworkCore;
using servartur.Entities;
using servartur.Enums;
using servartur.Exceptions;
using servartur.Models.Outgoing;
using servartur.Services;

namespace servartur.Tests.InfoServiceTests;

internal class GetFilteredPlayersTests
{
    private static DbContextOptions<GameDbContext> getDbOptions()
    {
        return new DbContextOptionsBuilder<GameDbContext>()
                        .UseInMemoryDatabase(databaseName: "test_db")
                        .Options;
    }

    private static PlayerInfoDto mapPlayer(Player p)
    {
        return new PlayerInfoDto
        {
            PlayerId = p.PlayerId,
            Nick = p.Nick ?? "",
        };
    }

    public static TheoryData<List<Player>, Predicate<Player>, List<PlayerInfoDto>> ValidTestCases()
    {
        var data = new TheoryData<List<Player>, Predicate<Player>, List<PlayerInfoDto>>();
        List<Player> allPlayers =
                [
                    new Player { PlayerId = 10, Nick = "player1", Team = Team.Good, Role = Role.GoodKnight },
                    new Player { PlayerId = 11, Nick = "player2", Team = Team.Evil, Role = Role.Morgana },
                    new Player { PlayerId = 12, Nick = "player2", Team = Team.Evil, Role = Role.Assassin },
                    new Player { PlayerId = 13, Nick = "player3", Team = Team.Good, Role = Role.Merlin },
                    new Player { PlayerId = 14, Nick = "player3", Team = Team.Good, Role = Role.Percival },
                ];
        List<PlayerInfoDto> goodPlayers = [mapPlayer(allPlayers[0]), mapPlayer(allPlayers[3]), mapPlayer(allPlayers[4])];
        List<PlayerInfoDto> evilPlayers = [mapPlayer(allPlayers[1]), mapPlayer(allPlayers[2])];

        data.Add(allPlayers, p => p.Team == Team.Good, goodPlayers);
        data.Add(allPlayers, p => p.Team == Team.Evil, evilPlayers);
        return data;
    }

    [Theory]
    [MemberData(nameof(ValidTestCases))]
    public void GetFilteredPlayersValidRoomIdAndPredicateReturnsFilteredPlayerList
        (List<Player> players, Predicate<Player> predicate, List<PlayerInfoDto> expectedDtos)
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<InfoService>>();
        var mapperMock = new Mock<IMapper>();
        var infoService = new InfoService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        var room = new Room { RoomId = 1, Players = players, };

        dbContextMock.Setup(db => db.Rooms).ReturnsDbSet([room]);
        mapperMock.Setup(m => m.Map<PlayerInfoDto>(It.IsAny<Player>())).Returns<Player>((p) => mapPlayer(p));

        // Act
        var result = infoService.GetFilteredPlayers(room.RoomId, predicate);

        // Assert
        result.Should().BeEquivalentTo(expectedDtos);
    }

    [Fact]
    public void GetFilteredPlayersInvalidRoomIdThrowsRoomNotFoundException()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<InfoService>>();
        var mapperMock = new Mock<IMapper>();
        var infoService = new InfoService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        var invalidRoomId = 999;
        dbContextMock.Setup(db => db.Rooms).ReturnsDbSet([]);

        // Act and Assert
        Action action = () => infoService.GetFilteredPlayers(invalidRoomId, p => p.Team == Team.Good);
        action.Should().Throw<RoomNotFoundException>();
    }
}

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

internal class GetPlayerByIdTests
{
    private static DbContextOptions<GameDbContext> getDbOptions()
    {
        return new DbContextOptionsBuilder<GameDbContext>()
                    .UseInMemoryDatabase(databaseName: "test_db")
                    .Options;
    }

    [Fact]
    public void GetPlayerByIdValidPlayerIdReturnsPlayerInfoDto()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<InfoService>>();
        var mapperMock = new Mock<IMapper>();
        var infoService = new InfoService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        var player = new Player()
        {
            PlayerId = 1,
            Nick = "test_nick",
            Team = Team.Evil,
            Role = Role.Assassin,
            RoomId = 1,
        };
        var expectedPlayerInfoDto = new PlayerInfoDto
        {
            PlayerId = player.PlayerId,
            Nick = player!.Nick,
        };

        dbContextMock.Setup(db => db.Players).ReturnsDbSet([player]);
        mapperMock.Setup(m => m.Map<PlayerInfoDto>(player)).Returns(expectedPlayerInfoDto);

        // Act
        var result = infoService.GetPlayerById(player.PlayerId);

        // Assert
        result.Should().BeEquivalentTo(expectedPlayerInfoDto);
    }

    [Fact]
    public void GetPlayerByIdInvalidPlayerIdThrowsPlayerNotFoundException()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<InfoService>>();
        var mapperMock = new Mock<IMapper>();
        var infoService = new InfoService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        var invalidPlayerId = 999;
        dbContextMock.Setup(db => db.Players).ReturnsDbSet([]);

        // Act and Assert
        void action() => infoService.GetPlayerById(invalidPlayerId);
        Assert.Throws<PlayerNotFoundException>(action);
    }
}

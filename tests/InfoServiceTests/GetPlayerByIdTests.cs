using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using servartur.Entities;
using servartur.Enums;
using servartur.Exceptions;
using servartur.Services;
using Moq.EntityFrameworkCore;
using servartur.Models.Outgoing;

namespace servartur.Tests.InfoServiceTests;
public class GetPlayerByIdTests
{
    private static DbContextOptions<GameDbContext> getDbOptions()
        => new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: "test_db")
                .Options;
    [Fact]
    public void GetPlayerById_ValidPlayerId_ReturnsPlayerInfoDto()
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
            Team = Team.Evil.ToString(),
            Role = Role.Assassin.ToString(),
        };

        dbContextMock.Setup(db => db.Players).ReturnsDbSet(new List<Player>() { player });
        mapperMock.Setup(m => m.Map<PlayerInfoDto>(player)).Returns(expectedPlayerInfoDto);

        // Act
        var result = infoService.GetPlayerById(player.PlayerId);

        // Assert
        result.Should().BeEquivalentTo(expectedPlayerInfoDto);
    }

    [Fact]
    public void GetPlayerById_InvalidPlayerId_ThrowsPlayerNotFoundException()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<InfoService>>();
        var mapperMock = new Mock<IMapper>();
        var infoService = new InfoService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        var invalidPlayerId = 999;
        dbContextMock.Setup(db => db.Players).ReturnsDbSet(new List<Player>());

        // Act and Assert
        Action action = () => infoService.GetPlayerById(invalidPlayerId);
        Assert.Throws<PlayerNotFoundException>(action);
    }
}

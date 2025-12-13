using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq.EntityFrameworkCore;
using servartur.Entities;
using servartur.Enums;
using servartur.Exceptions;
using servartur.Models.Incoming;
using servartur.Services;

namespace servartur.Tests.MatchupServiceTests;

public class SetNicknameTests
{
    private static DbContextOptions<GameDbContext> getDbOptions()
        => new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: "test_db")
            .Options;

    [Fact]
    public void SetNickname_ValidDto_ShouldUpdatePlayerNicknameAndSaveChanges()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        const int playerId = 1;
        string newNick = "new_nick";
        var dto = new PlayerNicknameSetDto() { RoomId = -1, PlayerId = playerId, Nick = newNick };
        var player = new Player() { PlayerId = playerId, Nick = "old_nick" };

        dbContextMock.SetupGet(x => x.Players).ReturnsDbSet(new[] { player });

        // Act
        matchupService.SetNickname(dto);

        // Assert
        player.Nick.Should().Be(newNick);
        dbContextMock.Verify(db => db.SaveChanges(), Times.Once);
    }

    [Fact]
    public void SetNickname_PlayerNotFound_ThrowsPlayerNotFoundException()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        const int playerId = 100;
        var dto = new PlayerNicknameSetDto() { RoomId = -1, PlayerId = playerId, Nick = "new_nick" };

        dbContextMock.Setup(x => x.Players).ReturnsDbSet(Array.Empty<Player>());

        // Act and Assert
        Action action = () => matchupService.SetNickname(dto);
        Assert.Throws<PlayerNotFoundException>(action);
    }
}

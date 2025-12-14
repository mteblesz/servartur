using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq.EntityFrameworkCore;
using servartur.Entities;
using servartur.Exceptions;
using servartur.Models.Incoming;
using servartur.Services;

namespace servartur.Tests.MatchupServiceTests;

#pragma warning disable CA1515 // Consider making public types internal
public class SetNicknameTests
#pragma warning restore CA1515 // Consider making public types internal
{
    private static DbContextOptions<GameDbContext> getDbOptions()
    {
        return new DbContextOptionsBuilder<GameDbContext>()
                    .UseInMemoryDatabase(databaseName: "test_db")
                .Options;
    }

    [Fact]
    public void SetNicknameValidDtoShouldUpdatePlayerNicknameAndSaveChanges()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        const int playerId = 1;
        var newNick = "new_nick";
        var dto = new PlayerNicknameSetDto() { RoomId = -1, PlayerId = playerId, Nick = newNick };
        var player = new Player() { PlayerId = playerId, Nick = "old_nick" };

        dbContextMock.SetupGet(x => x.Players).ReturnsDbSet([player]);

        // Act
        matchupService.SetNickname(dto);

        // Assert
        player.Nick.Should().Be(newNick);
        dbContextMock.Verify(db => db.SaveChanges(), Times.Once);
    }

    [Fact]
    public void SetNicknamePlayerNotFoundThrowsPlayerNotFoundException()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        const int playerId = 100;
        var dto = new PlayerNicknameSetDto() { RoomId = -1, PlayerId = playerId, Nick = "new_nick" };

        dbContextMock.Setup(x => x.Players).ReturnsDbSet([]);

        // Act and Assert
        void action() => matchupService.SetNickname(dto);
        Assert.Throws<PlayerNotFoundException>(action);
    }
}

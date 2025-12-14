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

#pragma warning disable CA1515 // Consider making public types internal
public class GetRoomByIdTests
#pragma warning restore CA1515 // Consider making public types internal
{
    private static DbContextOptions<GameDbContext> getDbOptions()
    {
        return new DbContextOptionsBuilder<GameDbContext>()
                    .UseInMemoryDatabase(databaseName: "test_db")
                    .Options;
    }

    [Fact]
    public void GetRoomByIdValidRoomIdReturnsRoomInfoDto()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<InfoService>>();
        var mapperMock = new Mock<IMapper>();
        var infoService = new InfoService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        var roomId = 1;
        var roomStatus = RoomStatus.Matchup;

        var room = new Room()
        {
            RoomId = roomId,
            Status = roomStatus,
            Players = []
        };
        var expectedRoomInfoDto = new RoomInfoDto
        {
            RoomId = roomId,
            Status = roomStatus.ToString(),
            CurrentSquadId = null,
            Players = []
        };

        dbContextMock.Setup(db => db.Rooms).ReturnsDbSet([room]);
        mapperMock.Setup(m => m.Map<RoomInfoDto>(room)).Returns(expectedRoomInfoDto);

        // Act
        var result = infoService.GetRoomById(roomId);

        // Assert
        result.Should().BeEquivalentTo(expectedRoomInfoDto);
    }

    [Fact]
    public void GetRoomByIdInvalidRoomIdThrowsRoomNotFoundException()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<InfoService>>();
        var mapperMock = new Mock<IMapper>();
        var infoService = new InfoService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        var invalidRoomId = 999;
        dbContextMock.Setup(db => db.Rooms).ReturnsDbSet([]);

        // Act and Assert
        void action() => infoService.GetRoomById(invalidRoomId);
        Assert.Throws<RoomNotFoundException>(action);
    }
}

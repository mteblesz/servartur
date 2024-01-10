using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using servartur.Entities;
using servartur.Enums;
using servartur.Models;
using servartur.Services;
using Moq.EntityFrameworkCore;

namespace servartur.Tests.MatchupServiceTests;
public class MatchupServiceCreateRoomTests
{
    private static DbContextOptions<GameDbContext> getDbOptions()
        => new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateRoom_ValidDto")
                .Options;

    [Fact]
    public void CreateRoom_ValidDto_ReturnsRoomIdAndAddRoomToDB()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        var createRoomDto = new CreateRoomDto();
        var expectedRoomId = 1;
        var room = new Room { RoomId = expectedRoomId, Status = RoomStatus.Matchup };

        mapperMock.Setup(m => m.Map<Room>(It.IsAny<CreateRoomDto>())).Returns(room);
        dbContextMock.SetupGet(x => x.Rooms).ReturnsDbSet(new List<Room>());

        // Act
        var result = matchupService.CreateRoom(createRoomDto);

        // Assert
        result.Should().Be(expectedRoomId);

        // Ensure the room was added to the DbContext
        dbContextMock.Verify(db => db.Rooms.Add(It.IsAny<Room>()), Times.Once);
        dbContextMock.Verify(db => db.SaveChanges(), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(50)]
    public void CreateMultipleRooms_ValidDto_ReturnsRoomIdAndAddsRoomToDB(int numberOfCreations)
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<MatchupService>>();
        var mapperMock = new Mock<IMapper>();
        var matchupService = new MatchupService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        var createRoomDto = new CreateRoomDto();
        var expectedRoomIds = Enumerable.Range(1, numberOfCreations).ToList();
        IList<Room> rooms = Enumerable.Range(1, numberOfCreations)
            .Select(i => new Room { RoomId = i, Status = RoomStatus.Matchup }).ToList();

        dbContextMock.SetupGet(x => x.Rooms).ReturnsDbSet(new List<Room>());

        // Setup mapperMock to return subsequent rooms dynamically using RoomProvider.GetNext
        var roomProvider = new RoomProvider(rooms);
        mapperMock.Setup(m => m.Map<Room>(It.IsAny<CreateRoomDto>()))
            .Returns(() => roomProvider.GetNext());

        // Act
        var results = new List<int>();
        for (int i = 0; i < numberOfCreations; i++)
            results.Add(matchupService.CreateRoom(createRoomDto));

        // Assert
        results.Should().BeEquivalentTo(expectedRoomIds);
        dbContextMock.Verify(db => db.Rooms.Add(It.IsAny<Room>()), Times.Exactly(numberOfCreations));
        dbContextMock.Verify(db => db.SaveChanges(), Times.Exactly(numberOfCreations));
    }
    private class RoomProvider(IList<Room> rooms)
    {
        private readonly IList<Room> _rooms = rooms;
        private int i = 0;
        public Room GetNext() => _rooms[i++];
    }
}

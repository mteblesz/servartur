using Microsoft.EntityFrameworkCore;
using servartur.Entities;
using Moq.EntityFrameworkCore;

namespace servartur.Tests.MatchupServiceTests;

public class MatchupServiceStartGameTests
{
    private static DbContextOptions<GameDbContext> getDbOptions()
        => new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateRoom_ValidDto")
                .Options;

    // TODO
}

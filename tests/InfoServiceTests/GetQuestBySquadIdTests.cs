﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using servartur.Entities;
using servartur.Enums;
using servartur.Exceptions;
using servartur.Services;
using Moq.EntityFrameworkCore;
using servartur.Models.Outgoing;

namespace servartur.Tests.InfoServiceTests;
public class GetQuestBySquadIdTests
{
    private static DbContextOptions<GameDbContext> getDbOptions()
        => new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: "test_db")
                .Options;
    [Fact]
    public void GetQuestById_ValidSquadId_ReturnsSquadInfoDto()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<InfoService>>();
        var mapperMock = new Mock<IMapper>();
        var infoService = new InfoService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        var leader = new Player { PlayerId = 10, Nick = "leader", Team = Team.Good, Role = Role.GoodKnight };
        var evilEntity = new Player { PlayerId = 11, Nick = "evil_entity", Team = Team.Evil, Role = Role.EvilEntity };
        var squad = new Squad
        {
            SquadId = 1,
            QuestNumber = 1,
            SquadNumber = 1,
            RequiredMembersNumber = 2,
            Status = SquadStatus.Failed,
            LeaderId = leader.PlayerId,
            Leader = leader,
            RoomId = 100,
        };
        squad.Memberships =
            [
                new Membership { SquadId = 1, Squad = squad, PlayerId = leader.PlayerId, Player = leader },
                new Membership { SquadId = 1, Squad = squad, PlayerId = evilEntity.PlayerId, Player =  evilEntity },
            ];

        PlayerInfoDto leaderDto = new PlayerInfoDto{ PlayerId = leader.PlayerId, Nick = "leader" };
        List<PlayerInfoDto> memberDtos =
        [
            leaderDto,
            new () { PlayerId = evilEntity.PlayerId, Nick = "evil_entity"},
        ];
        var expectedQuestInfoDto = new QuestInfoDto
        {
            SquadId = squad.SquadId,
            QuestNumber = 1,
            SquadNumber = 1,
            RequiredMembersNumber = 2,
            Leader = leaderDto,
            Status = SquadStatus.Failed,
            Members = memberDtos,
            SquadVoteInfo = [],
            QuestVoteSuccessCount = 0,
        };

        dbContextMock.Setup(db => db.Squads).ReturnsDbSet(new List<Squad>() { squad });
        mapperMock.Setup(m => m.Map<QuestInfoDto>(It.IsAny<Squad>())).Returns(expectedQuestInfoDto);

        // Act
        var result = infoService.GetQuestBySquadId(squad.SquadId);

        // Assert
        result.Should().BeEquivalentTo(expectedQuestInfoDto);
    }

    [Fact]
    public void GetQuestById_InvalidSquadId_ThrowsSquadNotFoundException()
    {
        // Arrange
        var dbContextMock = new Mock<GameDbContext>(getDbOptions());
        var loggerMock = new Mock<ILogger<InfoService>>();
        var mapperMock = new Mock<IMapper>();
        var infoService = new InfoService(dbContextMock.Object, mapperMock.Object, loggerMock.Object);

        var invalidSquadId = 999;
        dbContextMock.Setup(db => db.Squads).ReturnsDbSet(new List<Squad>());

        // Act and Assert
        Action action = () => infoService.GetQuestBySquadId(invalidSquadId);
        Assert.Throws<SquadNotFoundException>(action);
    }
}

﻿using AutoMapper;
using servartur.Entities;
using servartur.Mappings;
using servartur.Enums;
using static servartur.DomainLogic.GameStartHelper;
using servartur.Models.Outgoing;

namespace servartur.Tests;
public class MappingProfileTests
{
    public static TheoryData<Squad, List<PlayerInfoDto>> ValidTestCases()
    {
        var data = new TheoryData<Squad, List<PlayerInfoDto>>();

        // 1st case
        var leader1 = new Player { PlayerId = 10, Nick = "leader", Team = Team.Good, Role = Role.GoodKnight };
        var evilEntity1 = new Player { PlayerId = 11, Nick = "evil_entity", Team = Team.Evil, Role = Role.EvilEntity };
        var squad1 = new Squad
        {
            SquadId = 1,
            QuestNumber = 1,
            RoundNumber = 1,
            RequiredPlayersNumber = 2,
            Status = SquadStatus.Failed,
            LeaderId = leader1.PlayerId,
            Leader = leader1,
            RoomId = 100,
        };
        squad1.Memberships =
            [
                new Membership { SquadId = 1, Squad = squad1, PlayerId = leader1.PlayerId, Player = leader1 },
                new Membership { SquadId = 1, Squad = squad1, PlayerId = evilEntity1.PlayerId, Player = evilEntity1 }
            ];

        List<PlayerInfoDto> memberDtos1 =
        [
            new() { PlayerId = leader1.PlayerId, Nick = "leader", Team = Team.Good.ToString(), Role = Role.GoodKnight.ToString() },
            new() { PlayerId = evilEntity1.PlayerId, Nick = "evil_entity", Team = Team.Evil.ToString(), Role = Role.EvilEntity.ToString() },
        ];
        data.Add(squad1, memberDtos1);

        // 2nd case
        var leader2 = new Player { PlayerId = 20, Nick = "leader", Team = Team.Good, Role = Role.Merlin };
        var goodknight1_2 = new Player { PlayerId = 21, Nick = "gk_1", Team = Team.Good, Role = Role.GoodKnight };
        var goodknight2_2 = new Player { PlayerId = 22, Nick = "gk_2", Team = Team.Good, Role = Role.GoodKnight };
        var squad2 = new Squad
        {
            SquadId = 2,
            QuestNumber = 2,
            RoundNumber = 2,
            RequiredPlayersNumber = 3,
            Status = SquadStatus.Successfull,
            LeaderId = leader2.PlayerId,
            Leader = leader2,
            RoomId = 100,
        };
        squad2.Memberships =
            [
                new Membership { SquadId = 1, Squad = squad2, PlayerId = leader2.PlayerId, Player = leader2 },
                new Membership { SquadId = 1, Squad = squad2, PlayerId = goodknight1_2.PlayerId, Player = goodknight1_2 },
                new Membership { SquadId = 1, Squad = squad2, PlayerId = goodknight2_2.PlayerId, Player = goodknight2_2 },
            ];

        List<PlayerInfoDto> memberDtos2 =
        [
            new PlayerInfoDto { PlayerId = leader2.PlayerId, Nick = "leader", Team = Team.Good.ToString(), Role = Role.Merlin.ToString() },
            new PlayerInfoDto { PlayerId = goodknight1_2.PlayerId, Nick = "gk_1", Team = Team.Good.ToString(), Role = Role.GoodKnight.ToString() },
            new PlayerInfoDto { PlayerId = goodknight2_2.PlayerId, Nick = "gk_2", Team = Team.Good.ToString(), Role = Role.GoodKnight.ToString() },
        ];
        data.Add(squad2, memberDtos2);

        return data;
    }

    [Theory]
    [MemberData(nameof(ValidTestCases))]
    public void SquadToSquadInfoDtoMapping_IsValid(Squad squad, List<PlayerInfoDto> expectedMemberDtos)
    {
        // Arrange
        var configuration = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
        var mapper = configuration.CreateMapper();

        // Act
        var squadInfoDto = mapper.Map<SquadInfoDto>(squad);

        // Assert
        squadInfoDto.Should().NotBeNull();
        squadInfoDto.SquadId.Should().Be(squad.SquadId);
        squadInfoDto.QuestNumber.Should().Be(squad.QuestNumber);
        squadInfoDto.RoundNumber.Should().Be(squad.RoundNumber);
        squadInfoDto.RequiredPlayersNumber.Should().Be(squad.RequiredPlayersNumber);
        squadInfoDto.Status.Should().Be(squad.Status);
        squadInfoDto.LeaderId.Should().Be(squad.LeaderId);
        squadInfoDto.Members.Should().NotBeNull();
        squadInfoDto.Members.Should().HaveCount(squad.Memberships.Count);
        squadInfoDto.Members.Should().BeEquivalentTo(expectedMemberDtos);
    }

}
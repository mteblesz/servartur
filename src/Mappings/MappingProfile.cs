using AutoMapper;
using servartur.Entities;
using servartur.DomainLogic;
using servartur.Models.Incoming;
using servartur.Models.Outgoing;

namespace servartur.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Room, RoomInfoDto>();

        CreateMap<StartGameDto, GameStartHelper.RoleInfo>();

        CreateMap<Player, PlayerInfoDto>();
        CreateMap<Player, PlayerRoleInfoDto>();


        CreateMap<SquadVote, VoteInfoDto>()
            .ForMember(dest => dest.VoterNick, opt => opt.MapFrom(
                src => src.Voter.Nick))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(
                src => src.Value));

        CreateMap<Squad, SquadInfoDto>()
            .ForMember(dest => dest.Members, opt => opt.MapFrom(
                src => src.Memberships.Select(m => m.Player) ?? Enumerable.Empty<Player>()));

        CreateMap<Squad, QuestInfoShortDto>();

        CreateMap<Squad, QuestInfoDto>()
            .ForMember(dest => dest.Members, opt => opt.MapFrom(
                src => src.Memberships.Select(m => m.Player) ?? Enumerable.Empty<Player>()))
            .ForMember(dest => dest.SquadVoteInfo, opt => opt.MapFrom(
                src => src.SquadVotes))
            .ForMember(dest => dest.QuestVoteSuccessCount, opt => opt.MapFrom(
                src => src.QuestVotes.Count(qv => qv.Value)));

        CreateMap<CastVoteDto, SquadVote>();
        CreateMap<CastVoteDto, QuestVote>();


        CreateMap<Room, EndGameInfoDto>();
        CreateMap<KillPlayerDto, Assassination>();
    }
}

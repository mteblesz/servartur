using AutoMapper;
using servartur.Entities;
using servartur.Models;
using servartur.Enums;
using servartur.DomainLogic;

namespace servartur.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Room, RoomInfoDto>();

        CreateMap<StartGameDto, GameStartHelper.RoleInfo>();

        CreateMap<Player, PlayerInfoDto>();
        CreateMap<Player, PlayerRoleInfoDto>();
        CreateMap<Squad, SquadInfoDto>()
            .ForMember(dest => dest.Members, opt => opt.MapFrom(
                src => 
                    src.Memberships.Select(m => m.Player)
                ));

        CreateMap<VoteDto, SquadVote>();
        CreateMap<VoteDto, QuestVote>();
    }
}

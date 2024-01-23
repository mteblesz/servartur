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
        CreateMap<Room, RoomInfoDto>()
            .ForMember(dest => dest.IsFull, opt => opt.MapFrom(src 
            => src.Players.Count >= GameCountsCalculator.MaxNumberOfPlayers
            ));

        CreateMap<StartGameDto, GameStartHelper.RoleInfo>();

        CreateMap<Player, PlayerInfoDto>();
        CreateMap<Squad, SquadInfoDto>()
            .ForMember(dest => dest.Members, opt => opt.MapFrom(
                src => 
                    src.Memberships.Select(m => m.Player)
                ));

        CreateMap<VoteDto, SquadVote>();
        CreateMap<VoteDto, QuestVote>();
    }
}

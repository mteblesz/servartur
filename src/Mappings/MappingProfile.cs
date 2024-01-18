﻿using AutoMapper;
using servartur.Entities;
using servartur.Models;
using servartur.Enums;
using servartur.DomainLogic;

namespace servartur.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Room, RoomDto>()
            .ForMember(dest => dest.IsFull, opt => opt.MapFrom(src 
            => src.Players.Count >= GameCountsCalculator.MaxNumberOfPlayers
            ));

        CreateMap<Player, PlayerDto>();

        CreateMap<StartGameDto, GameStartHelper.RoleInfo>();
    }
}

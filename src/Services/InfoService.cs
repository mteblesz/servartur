﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using servartur.Algorithms;
using servartur.Entities;
using servartur.Enums;
using servartur.Exceptions;
using servartur.Models;
using System.Linq;

namespace servartur.Services;

public interface IInfoService
{
    
    RoomInfoDto GetRoomById(int roomId);
    PlayerInfoDto GetPlayerById(int playerId);
    PlayerRoleInfoDto GetRoleByPlayerId(int playerId);
    List<PlayerInfoDto> GetFilteredPlayers(int roomId, Predicate<Player> predicate, Func<Player, Player>? obfuscate = null);
    List<PlayerInfoDto> GetKnownByPercivalPlayers(int roomId);
    SquadInfoDto GetSquadById(int squadId);
}
public class InfoService : IInfoService
{
    private readonly GameDbContext _dbContext;
    private readonly IMapper _mapper;
    public readonly ILogger<InfoService> _logger;

    public InfoService(GameDbContext dbContext, IMapper mapper, ILogger<InfoService> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public RoomInfoDto GetRoomById(int roomId)
    {
        var room = _dbContext.Rooms
            .Include(r => r.Players)
            .FirstOrDefault(r => r.RoomId == roomId)
            ?? throw new RoomNotFoundException(roomId);

        var result = _mapper.Map<RoomInfoDto>(room);
        return result;
    }

    public PlayerInfoDto GetPlayerById(int playerId)
    {
        var player = _dbContext.Players
            .FirstOrDefault(p => p.PlayerId == playerId)
            ?? throw new PlayerNotFoundException(playerId);

        var result = _mapper.Map<PlayerInfoDto>(player);
        return result;
    }

    public PlayerRoleInfoDto GetRoleByPlayerId(int playerId)
    {
        var player = _dbContext.Players
            .FirstOrDefault(p => p.PlayerId == playerId)
            ?? throw new PlayerNotFoundException(playerId);

        var result = _mapper.Map<PlayerRoleInfoDto>(player);
        return result;
    }

    /// <summary>
    /// Filters players in a specific room, by a predicate. May alter players with obfuscate func
    /// Use: getting a list of evil or good players in the room. Obfucate Oberon or Mordred for player info
    /// </summary>
    /// <param name="roomId"></param>
    /// <param name="predicate"> Flilter players list by this predicate</param>
    /// <param name="obfuscate"> Modify filtered players. When not provided, do Identity(player) </param> 
    /// <returns></returns>
    /// <exception cref="RoomNotFoundException"></exception>
    public List<PlayerInfoDto> GetFilteredPlayers(int roomId, Predicate<Player> filter, Func<Player, Player>? obfuscate = null)
    {
        var room = _dbContext.Rooms
            .Include(r => r.Players)
            .FirstOrDefault(r => r.RoomId == roomId)
            ?? throw new RoomNotFoundException(roomId);

        var filteredPlayers = room.Players.Where(p => filter(p)).ToList();
        if (obfuscate != null) filteredPlayers.Shuffle();
        obfuscate ??= p => p; 
        var obfuscatedPlayers = filteredPlayers.Select(p => obfuscate(p)).ToList();
        var result = obfuscatedPlayers.Select(p => _mapper.Map<PlayerInfoDto>(p)).ToList();
        return result;
    }
    public List<PlayerInfoDto> GetKnownByPercivalPlayers(int roomId)
    {
        var room = _dbContext.Rooms
            .Include(r => r.Players)
            .FirstOrDefault(r => r.RoomId == roomId)
            ?? throw new RoomNotFoundException(roomId);

        if (!room.Players.Any(p => p.Role == Role.Percival))
            throw new PercivalNotInGameException(roomId);

        Predicate<Player> predicate = p => p.Role == Role.Merlin || p.Role == Role.Morgana; 
        var filteredPlayers = room.Players.Where(p => predicate(p)).ToList();
        if (filteredPlayers.Count != 2)
            throw new PercivalButNoMerlinMorganaException(roomId);

        var result = filteredPlayers.Select(p => _mapper.Map<PlayerInfoDto>(p)).ToList();
        return result;
    }


    public SquadInfoDto GetSquadById(int squadId)
    {
        var squad = _dbContext.Squads
            .Include(s => s.Memberships)
            .ThenInclude(m => m.Player)
            .FirstOrDefault(p => p.SquadId == squadId)
            ?? throw new SquadNotFoundException(squadId);

        var result = _mapper.Map<SquadInfoDto>(squad);
        return result;
    }
}

﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using servartur.BussinessRules;
using servartur.Entities;
using servartur.Exceptions;
using servartur.Models;
using servartur.Types;
using servartur.Utils;

namespace servartur.Services;

public interface IMatchupService
{
    int CreatePlayer(CreatePlayerDto dto);
    RoomDto? GetById(int id);
    int CreateRoom([FromBody] CreateRoomDto dto);
    void RemovePlayer(int id);
    void MakeTeams(int roomId);
}

public class MatchupService : IMatchupService
{
    private readonly GameDbContext _dbContext;
    private readonly IMapper _mapper;
    public readonly ILogger<MatchupService> _logger;

    public MatchupService(GameDbContext dbContext, IMapper mapper, ILogger<MatchupService> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public int CreateRoom([FromBody] CreateRoomDto dto)
    {
        var room = _mapper.Map<Room>(dto);
        room.Status = RoomStatus.Matchup;

        _dbContext.Rooms.Add(room); 
        _dbContext.SaveChanges();
        return room.RoomId;
    }

    public RoomDto? GetById(int id)
    {
        var room = _dbContext
            .Rooms
            .Include(r => r.Players)
            .FirstOrDefault(r => r.RoomId == id);

        if (room == null) 
            return null;

        var result = _mapper.Map<RoomDto>(room);
        return result;
    }
    public int CreatePlayer(CreatePlayerDto dto)
    {
        if (!_dbContext.Rooms.Any(r => r.RoomId == dto.RoomId))
            throw new RoomNotFoundException(dto.RoomId);

        var player = _mapper.Map<Player>(dto);
        _dbContext.Players.Add(player);
        _dbContext.SaveChanges();

        return player.PlayerId;
    }
    public void RemovePlayer(int playerId)
    {
        var player = _dbContext
            .Players
            .FirstOrDefault(p => p.PlayerId == playerId);

        if (player == null)
            throw new PlayerNotFoundException(playerId);

        _dbContext.Players.Remove(player);
        _dbContext.SaveChanges();
    }

    // TODO remove this later in favour of startRoom()
    public void MakeTeams(int roomId)
    {
        var room = _dbContext
            .Rooms
            .Include(r => r.Players)
            .FirstOrDefault(r => r.RoomId == roomId);

        if (room == null)
            throw new RoomNotFoundException(roomId);

        var numberOfPlayers = room.Players.Count();
        int numberOfEvils = MatchupLogic.GetNumberOfEvilPlayers(numberOfPlayers);

        List<Team> teamAssignment = Enumerable.Range(0, numberOfPlayers)
            .Select(index => index < numberOfEvils ? Team.Evil : Team.Good)
            .ToList();
        teamAssignment.Shuffle();

        
        foreach (var player in room.Players)
        {
            player.Team = teamAssignment.First();
            teamAssignment.RemoveAt(0);
        }
        // TODO here assign Roles to players using RolesMapping class
        //DUMMY:
        List<Role?> roleAssignment = new() { Role.Merlin, Role.Assassin };
        roleAssignment.Insert(numberOfPlayers - 2, null);
        roleAssignment.Shuffle();
        foreach (var player in room.Players)
        {
            player.Role = roleAssignment.First();
            roleAssignment.RemoveAt(0);
        }

        //end dummy role assignment
        _dbContext.SaveChanges();
    }
}

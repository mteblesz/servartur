using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using servartur.DomainLogic;
using servartur.Entities;
using servartur.Exceptions;
using servartur.Models;
using servartur.Enums;
using servartur.Algorithms;
using Humanizer;
using System.Collections.ObjectModel;

namespace servartur.Services;

public interface IMatchupService
{
    int CreatePlayer(CreatePlayerDto dto);
    RoomDto? GetRoomById(int roomId);
    int CreateRoom([FromBody] CreateRoomDto dto);
    void RemovePlayer(int playerId);
    void StartGame(StartGameDto dto);
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

    public int CreateRoom(CreateRoomDto dto)
    {
        var room = _mapper.Map<Room>(dto);
        room.Status = RoomStatus.Matchup;

        _dbContext.Rooms.Add(room);
        _dbContext.SaveChanges();
        return room.RoomId;
    }

    public int CreatePlayer(CreatePlayerDto dto)
    {
        var room = _dbContext.Rooms
            .Include(r => r.Players)
            .FirstOrDefault(r => r.RoomId == dto.RoomId)
            ?? throw new RoomNotFoundException(dto.RoomId);
        if (room.Status != RoomStatus.Matchup)
            throw new RoomNotInMatchupException(dto.RoomId);
        if (room.Players.Count >= GameCountsCalculator.MaxNumberOfPLayers)
            throw new RoomIsFullException(dto.RoomId);

        var player = _mapper.Map<Player>(dto);
        _dbContext.Players.Add(player);
        _dbContext.SaveChanges();

        return player.PlayerId;
    }
    public void RemovePlayer(int playerId)
    {
        var player = _dbContext
            .Players
            .FirstOrDefault(p => p.PlayerId == playerId)
            ?? throw new PlayerNotFoundException(playerId);

        var room = _dbContext.Rooms.FirstOrDefault(r => r.RoomId == player.RoomId)
            ?? throw new RoomNotFoundException(player.RoomId);
        if (room.Status != RoomStatus.Matchup)
            throw new RoomNotInMatchupException(player.RoomId);

        _dbContext.Players.Remove(player);
        _dbContext.SaveChanges();
    }

    public RoomDto GetRoomById(int roomId)
    {
        var room = _dbContext
            .Rooms
            .Include(r => r.Players)
            .FirstOrDefault(r => r.RoomId == roomId)
            ?? throw new RoomNotFoundException(roomId);

        var result = _mapper.Map<RoomDto>(room);
        return result;
    }

    public void StartGame(StartGameDto dto)
    {
        var room = _dbContext
            .Rooms
            .Include(r => r.Players)
            .Include(r => r.Squads)
            .FirstOrDefault(r => r.RoomId == dto.RoomId)
            ?? throw new RoomNotFoundException(dto.RoomId);

        int playersCount = room.Players.Count;
        if (!GameCountsCalculator.IsPlayerCountValid(playersCount))
            throw new PlayerCountInvalidException(dto.RoomId);
        if (room.Status != RoomStatus.Matchup)
            throw new RoomNotInMatchupException(dto.RoomId);

        // Assign roles
        List<Role> roles = makeRoleDeck(dto, playersCount);
        foreach (var player in room.Players)
        {
            player.Role = roles.Pop();
            player.Team = RoleTeamMapping.Map(player.Role.Value); // skip nullable 
        }

        // Create first Squad
        var firstSquadSize = GameCountsCalculator.GetSquadRequiredSize(playersCount, 1);
        var firstSquad = new Squad()
        {
            Leader = room.Players.First(),
            QuestNumber = 1,
            RoundNumber = 1,
            RequiredPlayersNumber = firstSquadSize,
            Status = SquadStatus.SquadVoting,
        };
        room.Squads.Add(firstSquad);
        room.CurrentSquad = firstSquad;

        // Update room
        room.Status = RoomStatus.Playing;
        _dbContext.SaveChanges();
    }

    private static List<Role> makeRoleDeck(StartGameDto dto, int playersCount)
    {
        var roles = new List<Role>();
        if (dto.AreMerlinAndAssassinInGame)
            roles.AddRange([Role.Merlin, Role.Assassin]);
        if (dto.ArePercivalAreMorganaInGame)
            roles.AddRange([Role.Percival, Role.Morgana]);
        if (dto.AreOberonAndMordredInGame)
            roles.AddRange([Role.Oberon, Role.Mordred]);

        int evilRolesCount = roles.Count(r => RoleTeamMapping.Map(r) == Team.Evil);
        int goodRolesCount = roles.Count(r => RoleTeamMapping.Map(r) == Team.Good);
        int evilPlayersTargetCount = GameCountsCalculator.GetEvilPlayersCount(playersCount);
        int goodPlayersTargetCount = playersCount - evilPlayersTargetCount;

        if (evilRolesCount <= evilPlayersTargetCount)
            throw new TooManyEvilRolesException();

        roles.AddRange(Enumerable.Repeat(Role.EvilEntity, evilPlayersTargetCount - evilRolesCount));
        roles.AddRange(Enumerable.Repeat(Role.GoodKnight, goodPlayersTargetCount - goodRolesCount));

        roles.Shuffle();
        return roles;
    }
}

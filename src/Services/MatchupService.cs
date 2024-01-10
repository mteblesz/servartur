using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using servartur.DomainLogic;
using servartur.Entities;
using servartur.Exceptions;
using servartur.Models;
using servartur.Enums;
using servartur.Algorithms;

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
        var player = _dbContext.Players
            .FirstOrDefault(p => p.PlayerId == playerId)
            ?? throw new PlayerNotFoundException(playerId);

        var room = _dbContext.Rooms.FirstOrDefault(r => r.RoomId == player.RoomId); 
        if (room!.Status != RoomStatus.Matchup)
            throw new RoomNotInMatchupException(player.RoomId);

        _dbContext.Players.Remove(player);
        _dbContext.SaveChanges();
    }

    public RoomDto GetRoomById(int roomId)
    {
        var room = _dbContext.Rooms
            .Include(r => r.Players)
            .FirstOrDefault(r => r.RoomId == roomId)
            ?? throw new RoomNotFoundException(roomId);

        var result = _mapper.Map<RoomDto>(room);
        return result;
    }

    public void StartGame(StartGameDto dto)
    {
        var room = _dbContext.Rooms
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
        var roleInfo = _mapper.Map<GameStartHelper.RoleInfo>(dto);
        var roles = GameStartHelper.MakeRoleDeck(playersCount, roleInfo, out bool tooManyEvilRoles);
        if (tooManyEvilRoles)
            throw new TooManyEvilRolesException();

        foreach (var player in room.Players)
        {
            player.Role = roles.Pop();
            player.Team = RoleTeamMapping.Map(player.Role.Value); // skip nullable 
        }

        // Create first Squad
        var firstSquad = GameStartHelper.MakeFirstSquad(room.Players.First(), playersCount);
        room.Squads.Add(firstSquad);
        room.CurrentSquad = firstSquad;

        // Update room
        room.Status = RoomStatus.Playing;
        _dbContext.SaveChanges();
    }
}

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
    RoomDto GetRoomById(int roomId);
    int CreateRoom();
    int JoinRoom(int roomId);
    void SetNickname(PlayerNicknameSetDto dto);
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

    public RoomDto GetRoomById(int roomId)
    {
        var room = _dbContext.Rooms
            .Include(r => r.Players)
            .FirstOrDefault(r => r.RoomId == roomId)
            ?? throw new RoomNotFoundException(roomId);

        var result = _mapper.Map<RoomDto>(room);
        return result;
    }

    public int CreateRoom()
    {
        var room = new Room { Status = RoomStatus.Matchup };

        _dbContext.Rooms.Add(room);
        _dbContext.SaveChanges();
        return room.RoomId;
    }

    public int JoinRoom(int roomId)
    {
        var room = _dbContext.Rooms
            .Include(r => r.Players)
            .FirstOrDefault(r => r.RoomId == roomId)
            ?? throw new RoomNotFoundException(roomId);
        if (room.Status != RoomStatus.Matchup)
            throw new RoomNotInMatchupException(roomId);
        if (room.Players.Count >= GameCountsCalculator.MaxNumberOfPlayers)
            throw new RoomIsFullException(roomId);

        var player = new Player() { RoomId = roomId };
        _dbContext.Players.Add(player);
        _dbContext.SaveChanges();

        return player.PlayerId;
    }

    public void SetNickname(PlayerNicknameSetDto dto)
    {
        var player = _dbContext.Players
            .FirstOrDefault(p => p.PlayerId == dto.PlayerId)
            ?? throw new PlayerNotFoundException(dto.PlayerId);

        player.Nick = dto.Nick;
        _dbContext.SaveChanges();
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

        var roleInfo = _mapper.Map<GameStartHelper.RoleInfo>(dto);
        var roles = GameStartHelper.MakeRoleDeck(playersCount, roleInfo, out bool tooManyEvilRoles);
        if (tooManyEvilRoles)
            throw new TooManyEvilRolesException();

        // Assign roles
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

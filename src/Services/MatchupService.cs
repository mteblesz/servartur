using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using servartur.DomainLogic;
using servartur.Entities;
using servartur.Exceptions;
using servartur.Enums;
using servartur.Utils;
using servartur.Models.Outgoing;
using servartur.Models.Incoming;

namespace servartur.Services;

public interface IMatchupService
{
    int CreateRoom();
    int JoinRoom(int roomId);
    void SetNickname(PlayerNicknameSetDto dto);
    void RemovePlayer(int playerId);
    void StartGame(StartGameDto dto);
    List<PlayerInfoDto> GetUpdatedPlayers(int roomId);
    SquadInfoDto GetUpdatedCurrentSquad(int roomId);
    List<QuestInfoShortDto> GetUpdatedQuestsSummary(int roomId);
}

public class MatchupService : DataUpdatesService, IMatchupService
{ 
    public MatchupService(GameDbContext dbContext, IMapper mapper, ILogger<MatchupService> logger) 
        : base(dbContext, mapper, logger) { }

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
        Random random = new Random();
        var leader = room.Players[random.Next(room.Players.Count)];
        var firstSquad = GameStartHelper.MakeFirstSquad(leader, playersCount);
        room.Squads.Add(firstSquad);
        room.CurrentSquad = firstSquad;

        // Update room
        room.Status = RoomStatus.Playing;
        _dbContext.SaveChanges();
    }
}

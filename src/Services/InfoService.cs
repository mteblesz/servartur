using AutoMapper;
using Microsoft.EntityFrameworkCore;
using servartur.Entities;
using servartur.Exceptions;
using servartur.Models;

namespace servartur.Services;

public interface IInfoService
{
    RoomInfoDto GetRoomById(int roomId);
    PlayerInfoDto GetPlayerById(int playerId);
    List<PlayerInfoDto> GetFilteredPlayers(int roomId, Predicate<Player> predicate);
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

    /// <summary>
    /// Filters players in a specific room, by a predicate.
    /// Use: getting a list of evil or good players in the room.
    /// </summary>
    /// <param name="roomId"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    /// <exception cref="RoomNotFoundException"></exception>
    public List<PlayerInfoDto> GetFilteredPlayers(int roomId, Predicate<Player> predicate)
    {
        var room = _dbContext.Rooms
            .Include(r => r.Players)
            .FirstOrDefault(r => r.RoomId == roomId)
            ?? throw new RoomNotFoundException(roomId);

        var filteredPlayers = room.Players.Where(p => predicate(p)).ToList();
        var result = filteredPlayers.Select(p => _mapper.Map<PlayerInfoDto>(p)).ToList();
        return result;
    }
}

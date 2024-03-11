using AutoMapper;
using Microsoft.EntityFrameworkCore;
using servartur.Entities;
using servartur.Exceptions;
using servartur.Models.Outgoing;

namespace servartur.Services;

/// <summary>
/// Exposes methods for getting updated data for Refreshes
/// </summary>
public abstract class DataUpdatesService : BaseService
{
    protected DataUpdatesService(GameDbContext dbContext, IMapper mapper, ILogger logger) 
        : base(dbContext, mapper, logger)
    {}

    public List<PlayerInfoDto> GetUpdatedPlayers(int roomId)
    {
        var room = _dbContext.Rooms
            .Include(r => r.Players)
            .FirstOrDefault(r => r.RoomId == roomId)
            ?? throw new RoomNotFoundException(roomId);

        var players = _mapper.Map<List<PlayerInfoDto>>(room.Players);
        return players.OrderBy(player => player.PlayerId).ToList();
    }

    public List<SquadInfoDto> GetUpdatedSquads(int roomId)
    {
        var room = _dbContext.Rooms
            .Include(r => r.Squads)
            .FirstOrDefault(r => r.RoomId == roomId)
            ?? throw new RoomNotFoundException(roomId);

        var squads = _mapper.Map<List<SquadInfoDto>>(room.Squads);
        return squads.OrderBy(s => s.SquadId).ToList();
    }
}

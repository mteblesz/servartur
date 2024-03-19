using AutoMapper;
using Microsoft.EntityFrameworkCore;
using servartur.Utils;
using servartur.Entities;
using servartur.Enums;
using servartur.Exceptions;
using servartur.Models.Outgoing;

namespace servartur.Services;

public interface IGameService
{
    PlayerInfoDto LeaveGame(int playerId, out int roomId);
    List<PlayerInfoDto> GetUpdatedPlayers(int roomId);
}
public class GameService : DataUpdatesService, IGameService
{
    public GameService(GameDbContext dbContext, IMapper mapper, ILogger<SquadService> logger)
        : base(dbContext, mapper, logger) { }

    public PlayerInfoDto LeaveGame(int playerId, out int roomId)
    {
        var player = _dbContext.Players
            .Include(p => p.Room)
            .FirstOrDefault(p => p.PlayerId == playerId)
            ?? throw new PlayerNotFoundException(playerId);

        var room = player.Room;
        if (room!.Status == RoomStatus.Unknown)
            throw new RoomInBadStateException(player.RoomId);

        // don't change anythng in db, no new quest can be started,
        // other must wait for this player to reconnect somehow (TODO),
        // for now, others get notified via signalr

        roomId = player.RoomId;
        return _mapper.Map<PlayerInfoDto>(player);
    }
}

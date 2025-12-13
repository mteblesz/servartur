using AutoMapper;
using Microsoft.EntityFrameworkCore;
using servartur.Entities;
using servartur.Exceptions;
using servartur.Models.Incoming;
using servartur.Models.Outgoing;

namespace servartur.Services;

internal interface IKillService
{
    EndGameInfoDto GetUpdatedEndGame(int roomId);
    void KillPlayer(KillPlayerDto dto, out int roomId);
}
internal class KillService : DataUpdatesService, IKillService
{
    public KillService(GameDbContext dbContext, IMapper mapper, ILogger<KillService> logger)
        : base(dbContext, mapper, logger) { }

    public void KillPlayer(KillPlayerDto dto, out int roomId)
    {
        var room = _dbContext.Rooms
            .Include(r => r.Players)
            .FirstOrDefault(r => r.RoomId == dto.RoomId)
            ?? throw new RoomNotFoundException(dto.RoomId);

        var assassin = room.Players
            .FirstOrDefault(p => p.PlayerId == dto.AssassinId)
            ?? throw new PlayerNotFoundException(dto.RoomId);
        if (assassin.Role != Enums.Role.Assassin)
        {
            throw new PlayerIsNotAssassinException(assassin.PlayerId);
        }

        var target = room.Players
            .FirstOrDefault(p => p.PlayerId == dto.TargetId)
            ?? throw new PlayerNotFoundException(dto.RoomId);

        var assassination = _mapper.Map<Assassination>(dto);
        assassination.Result = target.Role == Enums.Role.Merlin;
        room.Assassination = assassination;
        _dbContext.SaveChanges();

        roomId = room.RoomId;
    }
}

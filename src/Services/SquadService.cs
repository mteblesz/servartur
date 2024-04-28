using AutoMapper;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using servartur.DomainLogic;
using servartur.Entities;
using servartur.Enums;
using servartur.Exceptions;
using servartur.Models.Outgoing;

namespace servartur.Services;

public interface ISquadService
{
    void AddMember(int playerId, out int roomId);
    void RemoveMember(int playerId, out int roomId);
    void SubmitSquad(int squadId, out int roomId);
    SquadInfoDto GetUpdatedCurrentSquad(int roomId);
    List<QuestInfoShortDto> GetUpdatedQuestsSummary(int roomId);
}
public class SquadService : DataUpdatesService, ISquadService
{
    public SquadService(GameDbContext dbContext, IMapper mapper, ILogger<SquadService> logger)
        : base(dbContext, mapper, logger) { }

    public void AddMember(int playerId, out int roomId)
    {
        var player = _dbContext.Players
            .Include(p => p.Room)
                .ThenInclude(r => r.CurrentSquad )
                    .ThenInclude(s => s.Memberships)
            .FirstOrDefault(p => p.PlayerId == playerId)
            ?? throw new PlayerNotFoundException(playerId);
        if (player.Room.Status != RoomStatus.Playing)
            throw new RoomInBadStateException(player.RoomId);
        if (player.Room.CurrentSquad == null)
            throw new RoomInBadStateException(player.RoomId);

        var currentSquad = player.Room.CurrentSquad;
        if (currentSquad.Memberships.Count >= currentSquad.RequiredPlayersNumber)
            throw new SquadIsFullException(player.RoomId);

        _dbContext.Memberships.Add(new Membership { Player = player, Squad = currentSquad });
        roomId = player.RoomId;
        _dbContext.SaveChanges();
    }

    public void RemoveMember(int playerId, out int roomId)
    {
        var player = _dbContext.Players
            .Include(p => p.Room)
                .ThenInclude(r => r.CurrentSquad)
                    .ThenInclude(s => s.Memberships)
            .FirstOrDefault(p => p.PlayerId == playerId)
            ?? throw new PlayerNotFoundException(playerId);
        if (player.Room.Status != RoomStatus.Playing)
            throw new RoomInBadStateException(player.RoomId);
        if (player.Room.CurrentSquad == null)
            throw new RoomInBadStateException(player.RoomId);

        var currentSquad = player.Room.CurrentSquad;
        if (currentSquad.Memberships.Count <= 0)
            throw new SquadIsEmptyException(player.RoomId);

        var membershipToBeRemoved = _dbContext.Memberships
            .FirstOrDefault(m => m.PlayerId == playerId && m.SquadId == currentSquad.SquadId);
        if (membershipToBeRemoved != null)
        _dbContext.Memberships.Remove(membershipToBeRemoved);
        roomId = player.RoomId;
        _dbContext.SaveChanges();
    }

    public void SubmitSquad(int squadId, out int roomId)
    {
        throw new NotImplementedException();
    }
}

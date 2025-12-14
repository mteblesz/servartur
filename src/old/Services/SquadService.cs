using AutoMapper;
using Microsoft.EntityFrameworkCore;
using servartur.Entities;
using servartur.Enums;
using servartur.Exceptions;
using servartur.Models.Outgoing;

namespace servartur.Services;

internal interface ISquadService
{
    SquadInfoDto GetUpdatedCurrentSquad(int roomId);
#pragma warning disable CA1002 // Do not expose generic lists
    List<QuestInfoShortDto> GetUpdatedQuestsSummary(int roomId);
#pragma warning disable CA1021 // Avoid out parameters
    void AddMember(int playerId, out int roomId);
    void RemoveMember(int playerId, out int roomId);
    void SubmitSquad(int squadId, out int roomId);
}
internal class SquadService : DataUpdatesService, ISquadService
{
    public SquadService(GameDbContext dbContext, IMapper mapper, ILogger<SquadService> logger)
        : base(dbContext, mapper, logger) { }

    public void AddMember(int playerId, out int roomId)
    {
        var player = _dbContext.Players
            .Include(p => p.Room)
                .ThenInclude(r => r.CurrentSquad)
                    .ThenInclude(s => s!.Memberships)
            .FirstOrDefault(p => p.PlayerId == playerId)
            ?? throw new PlayerNotFoundException(playerId);
        if (player.Room.Status != RoomStatus.Playing)
        {
            throw new RoomInBadStateException(player.RoomId);
        }

        if (player.Room.CurrentSquad == null)
        {
            throw new RoomInBadStateException(player.RoomId);
        }

        var currentSquad = player.Room.CurrentSquad;
        if (currentSquad.Memberships.Count >= currentSquad.RequiredMembersNumber)
        {
            throw new SquadIsFullException(player.RoomId);
        }

        _dbContext.Memberships.Add(new Membership { Player = player, Squad = currentSquad });
        _dbContext.SaveChanges();
        roomId = player.RoomId;
    }

    public void RemoveMember(int playerId, out int roomId)
    {
        var player = _dbContext.Players
            .Include(p => p.Room)
                .ThenInclude(r => r.CurrentSquad)
                    .ThenInclude(s => s!.Memberships)
            .FirstOrDefault(p => p.PlayerId == playerId)
            ?? throw new PlayerNotFoundException(playerId);
        if (player.Room.Status != RoomStatus.Playing)
        {
            throw new RoomInBadStateException(player.RoomId);
        }

        if (player.Room.CurrentSquad == null)
        {
            throw new RoomInBadStateException(player.RoomId);
        }

        var currentSquad = player.Room.CurrentSquad;
        if (currentSquad.Memberships.Count <= 0)
        {
            throw new SquadIsEmptyException(player.RoomId);
        }

        var membershipToBeRemoved = _dbContext.Memberships
            .FirstOrDefault(m => m.PlayerId == playerId && m.SquadId == currentSquad.SquadId);
        if (membershipToBeRemoved != null)
        {
            _dbContext.Memberships.Remove(membershipToBeRemoved);
        }

        _dbContext.SaveChanges();
        roomId = player.RoomId;
    }

    public void SubmitSquad(int squadId, out int roomId)
    {
        var squad = _dbContext.Squads
            .Include(s => s.Memberships)
            .FirstOrDefault(s => s.SquadId == squadId)
            ?? throw new SquadNotFoundException(squadId);
        if (squad.Status != SquadStatus.SquadChoice)
        {
            throw new SquadInWrongStateException(squadId);
        }

        if (squad.Memberships.Count != squad.RequiredMembersNumber)
        {
            throw new SquadIsNotFullException(squadId);
        }

        squad.Status = SquadStatus.Submitted;
        _dbContext.SaveChanges();
        roomId = squad.RoomId;
    }

}

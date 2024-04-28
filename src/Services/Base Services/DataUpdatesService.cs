using AutoMapper;
using Microsoft.EntityFrameworkCore;
using servartur.DomainLogic;
using servartur.Entities;
using servartur.Enums;
using servartur.Exceptions;
using servartur.Models.Outgoing;
using System.Collections.Generic;

namespace servartur.Services;

/// <summary>
/// Exposes methods for getting updated data for Refreshes
/// </summary>
public abstract class DataUpdatesService : BaseService
{
    protected DataUpdatesService(GameDbContext dbContext, IMapper mapper, ILogger logger)
        : base(dbContext, mapper, logger)
    { }

    public List<PlayerInfoDto> GetUpdatedPlayers(int roomId)
    {
        var room = _dbContext.Rooms
            .Include(r => r.Players)
            .FirstOrDefault(r => r.RoomId == roomId)
            ?? throw new RoomNotFoundException(roomId);

        var players = _mapper.Map<List<PlayerInfoDto>>(room.Players);
        return players.OrderBy(player => player.PlayerId).ToList();
    }

    public SquadInfoDto GetUpdatedCurrentSquad(int roomId)
    {
        var room = _dbContext.Rooms
            .FirstOrDefault(r => r.RoomId == roomId)
            ?? throw new RoomNotFoundException(roomId);

        var squadId = room.CurrentSquadId;
        var squad = _dbContext.Squads
             .Include(s => s.Leader)
             .Include(s => s.Memberships)
                .ThenInclude(m => m.Player)
            .FirstOrDefault(r => r.RoomId == roomId)
            ?? throw new RoomNotFoundException(roomId);

        var result = _mapper.Map<SquadInfoDto>(squad);
        return result;
    }

    public List<QuestInfoShortDto> GetUpdatedQuestsSummary(int roomId)
    {
        var room = _dbContext.Rooms
            .Include(r => r.Players)
            .Include(r => r.Squads)
            .FirstOrDefault(r => r.RoomId == roomId)
            ?? throw new RoomNotFoundException(roomId);

        // Select finished quests
        var squads = room.Squads
            .Where(s =>
                    s.Status == SquadStatus.Successful
                    || s.Status == SquadStatus.Failed).ToList();
        // Add current squad
        squads.Add(room.Squads.First(s => s.SquadId == room.CurrentSquadId));

        // Map finished + current 
        var summary = _mapper.Map<List<QuestInfoShortDto>>(squads);
        // Add future squads
        summary.AddRange(getUpcomingQuestsInfo(room.Players.Count, squads.Count));

        // return (finished + current + future) quest info
        return summary.OrderBy(s => s.QuestNumber).ToList();
    }
    private static List<QuestInfoShortDto> getUpcomingQuestsInfo(int playersCount, int curentQuestNumber)
    {
        List<QuestInfoShortDto> result = [];
        for (int i = curentQuestNumber + 1; i <= 5; i++)
        {
            result.Add(new QuestInfoShortDto
            {
                SquadId = null,
                QuestNumber = i,
                RequiredMembersNumber = GameCountsCalculator.GetSquadRequiredSize(playersCount, i),
                IsDoubleFail = GameCountsCalculator.IsQuestDoubleFail(playersCount, i),
                Status = SquadStatus.Upcoming,
            });
        }
        return result;
    }
}

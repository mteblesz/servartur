using AutoMapper;
using Microsoft.EntityFrameworkCore;
using servartur.DomainLogic;
using servartur.Entities;
using servartur.Enums;
using servartur.Exceptions;
using servartur.Models.Outgoing;

namespace servartur.Services.BaseServices;

/// <summary>
/// Exposes methods for getting updated data for Refreshes
/// </summary>
internal abstract class DataUpdatesService : BaseService
{
    protected DataUpdatesService(GameDbContext dbContext, IMapper mapper, ILogger logger)
        : base(dbContext, mapper, logger)
    { }

#pragma warning disable CA1002 // Do not expose generic lists
    public List<PlayerInfoDto> GetUpdatedPlayers(int roomId)
#pragma warning restore CA1002 // Do not expose generic lists
    {
        var room = _dbContext.Rooms
            .Include(r => r.Players)
            .FirstOrDefault(r => r.RoomId == roomId)
            ?? throw new RoomNotFoundException(roomId);

        var players = _mapper.Map<List<PlayerInfoDto>>(room.Players);
        return [.. players.OrderBy(player => player.PlayerId)];
    }

    public SquadInfoDto GetUpdatedCurrentSquad(int roomId)
    {
        var room = _dbContext.Rooms
            .Include(r => r.CurrentSquad)
                .ThenInclude(s => s!.Leader)
            .Include(r => r.CurrentSquad)
                .ThenInclude(s => s!.Memberships)
                    .ThenInclude(m => m.Player)
            .FirstOrDefault(r => r.RoomId == roomId)
            ?? throw new RoomNotFoundException(roomId);

        var squad = room.CurrentSquad ?? throw new SquadNotFoundException(roomId);

        var result = _mapper.Map<SquadInfoDto>(squad);
        return result;
    }

#pragma warning disable CA1002 // Do not expose generic lists
    public List<QuestInfoShortDto> GetUpdatedQuestsSummary(int roomId)
#pragma warning restore CA1002 // Do not expose generic lists
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
        return [.. summary.OrderBy(s => s.QuestNumber)];
    }
    private static List<QuestInfoShortDto> getUpcomingQuestsInfo(int playerCount, int curentQuestNumber)
    {
        List<QuestInfoShortDto> result = [];
        for (var i = curentQuestNumber + 1; i <= 5; i++)
        {
            result.Add(new QuestInfoShortDto
            {
                SquadId = null,
                QuestNumber = i,
                RequiredMembersNumber = GameCountsCalculator.GetSquadRequiredSize(playerCount, i),
                IsDoubleFail = GameCountsCalculator.IsQuestDoubleFail(playerCount, i),
                Status = SquadStatus.Upcoming,
            });
        }
        return result;
    }

    public EndGameInfoDto GetUpdatedEndGame(int roomId)
    {
        var room = _dbContext.Rooms
            .FirstOrDefault(r => r.RoomId == roomId)
            ?? throw new RoomNotFoundException(roomId);

        var result = _mapper.Map<EndGameInfoDto>(room);
        return result;
    }
}

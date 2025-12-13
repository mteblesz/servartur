using AutoMapper;
using Microsoft.EntityFrameworkCore;
using servartur.DomainLogic;
using servartur.Entities;
using servartur.Enums;
using servartur.Exceptions;
using servartur.Models.Outgoing;

namespace servartur.Services.Base_Services;

/// <summary>
/// Exposes methods for getting updated data about votings for voting data refreshes
/// </summary>
public abstract class VotingUpdates : DataUpdatesService
{
    protected VotingUpdates(GameDbContext dbContext, IMapper mapper, ILogger logger)
        : base(dbContext, mapper, logger)
    { }

    public VotingSquadEndedInfoDto GetUpdatedSquadVotingEnded(int roomId)
    {
        return new VotingSquadEndedInfoDto
        {
            CurrentSquadInfo = GetUpdatedCurrentSquad(roomId),
            EndGameInfo = GetUpdatedEndGame(roomId),
        };
    }

    public VotingQuestEndedInfoDto GetUpdatedQuestVotingEnded(int roomId)
    {
        return new VotingQuestEndedInfoDto
        {
            CurrentSquadInfo = GetUpdatedCurrentSquad(roomId),
            QuestsSummary = GetUpdatedQuestsSummary(roomId),
            EndGameInfo = GetUpdatedEndGame(roomId),
        };
    }
}

using AutoMapper;
using servartur.Entities;
using servartur.Models.Outgoing;

namespace servartur.Services.BaseServices;

/// <summary>
/// Exposes methods for getting updated data about votings for voting data refreshes
/// </summary>
internal abstract class VotingUpdates : DataUpdatesService
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

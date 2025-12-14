namespace Servartur.Domain.Models.Votes;

public record QuestVote(bool Value, Guid SquadId, Guid PlayerId);

namespace Servartur.Domain.Models.Votes;

public record SquadVote(bool Value, Guid SquadId, Guid PlayerId);

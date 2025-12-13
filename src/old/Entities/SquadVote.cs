namespace servartur.Entities;

internal class SquadVote
{
    public int SquadVoteId { get; set; }
    public bool Value { get; set; }

    public int SquadId { get; set; }
    public virtual required Squad Squad { get; set; }
    public int VoterId { get; set; }
    public virtual required Player Voter { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace servartur.Entities;

public class SquadVote
{
    public int SquadVoteId { get; set; }
    public bool Value { get; set; }

    public int SquadId { get; set; }
    public required virtual Squad Squad { get; set; }
    public int VoterId { get; set; }
    public required virtual Player Voter { get; set; }
}

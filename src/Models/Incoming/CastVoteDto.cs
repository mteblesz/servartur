using System.ComponentModel.DataAnnotations;

namespace servartur.Models.Incoming;

public class CastVoteDto
{
    [Required]
    public bool Value { get; set; }
    [Required]
    public int SquadId { get; set; }
    [Required]
    public int VoterId { get; set; }
}

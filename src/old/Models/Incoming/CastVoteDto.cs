using System.ComponentModel.DataAnnotations;

namespace servartur.Models.Incoming;

internal class CastVoteDto
{
    [Required]
    public required bool Value { get; set; }
    [Required]
    public required int SquadId { get; set; }
    [Required]
    public required int VoterId { get; set; }
}

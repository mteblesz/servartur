using System.ComponentModel.DataAnnotations;

namespace servartur.Models.Incoming;

internal class KillPlayerDto
{
    [Required]
    public int RoomId { get; set; }

    [Required]
    public int AssassinId { get; set; }

    [Required]
    public int TargetId { get; set; }

}

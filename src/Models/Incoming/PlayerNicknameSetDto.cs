using System.ComponentModel.DataAnnotations;

namespace servartur.Models.Incoming;

public class PlayerNicknameSetDto
{
    [Required]
    public required int RoomId { get; set; }
    [Required]
    public required int PlayerId { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 3)]
    public required string Nick { get; set; }
}

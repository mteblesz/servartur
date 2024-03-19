using System.ComponentModel.DataAnnotations;

namespace servartur.Models.Incoming;

public class PlayerNicknameSetDto
{
    [Required]
    public int RoomId { get; set; }
    [Required]
    public int PlayerId { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 3)]
    public string Nick { get; set; } = null!;
}

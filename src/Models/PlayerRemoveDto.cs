using System.ComponentModel.DataAnnotations;

namespace servartur.Models;

public class PlayerRemoveDto
{
    [Required]
    public int RoomId { get; set; }
    [Required]
    public int PlayerId { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace servartur.Models;

public class RoomConnectionDto
{
    [Required]
    public int RoomId { get; set; }
    [Required] 
    public required string HubConnectionId { get; set; }
}

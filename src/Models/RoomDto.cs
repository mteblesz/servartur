namespace servartur.Models;

public class RoomDto
{
    public int RoomId { get; set; }
    public string Status { get; set; } = null!;
    public bool IsFull { get; set; }

    public List<PlayerDto> Players { get; set; } = null!;

}

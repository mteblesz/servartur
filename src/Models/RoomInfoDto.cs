namespace servartur.Models;

public class RoomInfoDto
{
    public int RoomId { get; set; }
    public string Status { get; set; } = null!;
    public bool IsFull { get; set; }

    public List<PlayerInfoDto> Players { get; set; } = null!;

}

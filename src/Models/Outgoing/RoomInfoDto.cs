namespace servartur.Models.Outgoing;

public class RoomInfoDto
{
    public int RoomId { get; set; }
    public string Status { get; set; } = null!;
    public int? CurrentSquadId { get; set; }

    public List<PlayerInfoDto> Players { get; set; } = null!;

}

using servartur.Entities;

namespace servartur.Models.Outgoing;

public class EndGameInfoDto
{
    public required int RoomId { get; set; }
    public required string Status { get; set; }
}

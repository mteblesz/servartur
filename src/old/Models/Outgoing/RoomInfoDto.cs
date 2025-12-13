namespace servartur.Models.Outgoing;

internal class RoomInfoDto
{
    public required int RoomId { get; set; }
    public required string Status { get; set; }
    public required int? CurrentSquadId { get; set; }

    public required List<PlayerInfoDto> Players { get; set; }

}

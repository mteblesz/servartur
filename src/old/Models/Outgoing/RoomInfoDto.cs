namespace servartur.Models.Outgoing;

internal class RoomInfoDto
{
    public required int RoomId { get; set; }
    public required string Status { get; set; }
    public required int? CurrentSquadId { get; set; }

#pragma warning disable CA1002 // Do not expose generic lists
    public required List<PlayerInfoDto> Players { get; set; }
#pragma warning restore CA1002 // Do not expose generic lists

}

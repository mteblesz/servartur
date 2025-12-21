namespace Servartur.Data.PostgreSQL.Rooms;

internal class RoomEntity
{
    public required Guid Id { get; init; }
    public required string Status { get; init; }
}

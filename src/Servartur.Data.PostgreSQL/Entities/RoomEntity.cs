namespace Servartur.Data.PostgreSQL.Entities;

internal class RoomEntity
{
    public required Guid Id { get; init; }
    public required string Status { get; init; }

    public ICollection<PlayerEntity> Players { get; init; } = [];
}

namespace Servartur.Data.PostgreSQL.Entities;

internal class PlayerEntity
{
    public required Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Character { get; init; }

    public required Guid RoomId { get; init; }
    public RoomEntity Room { get; init; } = null!;
}

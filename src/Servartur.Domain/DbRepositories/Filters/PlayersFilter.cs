namespace Servartur.Domain.DbRepositories.Filters;

public class PlayersFilter
{
    public IEnumerable<string>? Names { get; init; }
    public Guid? RoomId { get; init; }
}

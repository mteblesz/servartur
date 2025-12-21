using Servartur.Domain.Models;

namespace Servartur.Domain.DbRepositories;

public interface IRoomsDbRepository
{
    Task CreateRoomAsync(Room room, CancellationToken ct);
}

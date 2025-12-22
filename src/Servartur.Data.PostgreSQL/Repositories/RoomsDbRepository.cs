using Servartur.Data.PostgreSQL.Entities;
using Servartur.Domain.DbRepositories;
using Servartur.Domain.Models;

namespace Servartur.Data.PostgreSQL.Repositories;

internal class RoomsDbRepository(DatabaseContext dbContext) : IRoomsDbRepository
{
    private readonly DatabaseContext _dbContext = dbContext;

    public async Task CreateRoomAsync(Room room, CancellationToken ct)
    {
        var entity = new RoomEntity
        {
            Id = room.Id,
            Status = room.Status.ToString(),
        };

        await _dbContext.Rooms.AddAsync(entity, ct);

        await _dbContext.SaveChangesAsync(ct);
    }
}

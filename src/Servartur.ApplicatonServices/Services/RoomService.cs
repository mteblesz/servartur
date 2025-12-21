using Servartur.Domain.DbRepositories;
using Servartur.Domain.Models;
using Servartur.Domain.Models.Enums;

namespace Servartur.ApplicatonServices.Services;

public class RoomService(IRoomsDbRepository roomsDbRepository)
{
    private readonly IRoomsDbRepository _roomsDbRepository = roomsDbRepository;

    public async Task<Guid> CreateRoomAsync(CancellationToken ct)
    {
        var roomId = Guid.NewGuid();

        var room = new Room
        {
            Id = roomId,
            Status = RoomStatus.Matchup,
        };

        await _roomsDbRepository.CreateRoomAsync(room, ct);

        return roomId;
    }
}

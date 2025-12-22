using Servartur.Domain.DbRepositories;
using Servartur.Domain.DbRepositories.Filters;
using Servartur.Domain.Exceptions;
using Servartur.Domain.Models;
using Servartur.Domain.Models.Enums;

namespace Servartur.ApplicatonServices.Services;

public class MatchupService(
    IPlayersDbRepository playersDbRepository,
    IRoomsDbRepository roomsDbRepository)
{
    private readonly IPlayersDbRepository _playersDbRepository = playersDbRepository;
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

    public async Task<Guid> CreatePlayerAsync(string name, Guid roomId, CancellationToken ct)
    {
        await ValidateNameIsNotYetTaken(name, roomId, ct);

        var playerId = Guid.NewGuid();

        var room = new Player
        {
            Id = playerId,
            Name = name,
            RoomId = roomId,
        };

        await _playersDbRepository.CreatePlayerAsync(room, ct);

        return playerId;
    }

    private async Task ValidateNameIsNotYetTaken(string name, Guid roomId, CancellationToken ct)
    {
        var playerNameExists = await _playersDbRepository.HasPlayers(new PlayersFilter
        {
            Names = [name],
            RoomId = roomId
        }, ct);

        if (playerNameExists)
        {
            // TODO: add handling
            throw new NameAlreadyUsedException(name, roomId);
        }
    }
}

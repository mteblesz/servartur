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

        var player = new Player
        {
            Id = playerId,
            Name = name,
            RoomId = roomId,
        };

        await _playersDbRepository.CreatePlayerAsync(player, ct);

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

    public async Task RemovePlayerAsync(Guid roomId, Guid playerId, CancellationToken ct)
    {
        // TODO: valdiate player is in room

        await _playersDbRepository.DeletePlayersAsync(new PlayersFilter
        {
            PlayerId = playerId,
        }, ct);
    }

    public async Task StartGameAsync(Guid roomId, CancellationToken ct)
    {
        // TODO: add game rules validation

        //if (!dto.AreMerlinAndAssassinInGame && dto.ArePercivalAndMorganaInGame)
        //{
        //    ModelState.AddModelError("", "Morgana and Percival can't be present with Merlin and Assassin missing");
        //    return BadRequest(ModelState);
        //}
        //if (!dto.AreMerlinAndAssassinInGame && dto.AreOberonAndMordredInGame)
        //{
        //    ModelState.AddModelError("", "Oberon and Mordred can't be present with Merlin and Assassin missing");
        //    return BadRequest(ModelState);
        //}

        throw new NotImplementedException();
    }
}

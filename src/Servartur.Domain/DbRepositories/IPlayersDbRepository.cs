using Servartur.Domain.DbRepositories.Filters;
using Servartur.Domain.Models;

namespace Servartur.Domain.DbRepositories;

public interface IPlayersDbRepository
{
    Task CreatePlayerAsync(Player player, CancellationToken ct);

    Task<bool> HasPlayers(PlayersFilter filter, CancellationToken ct);
}
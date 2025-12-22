using Microsoft.EntityFrameworkCore;
using Servartur.Data.PostgreSQL.Entities;
using Servartur.Data.PostgreSQL.Repositories.FilterHelpers;
using Servartur.Domain.DbRepositories;
using Servartur.Domain.DbRepositories.Filters;
using Servartur.Domain.Models;

namespace Servartur.Data.PostgreSQL.Repositories;

internal class PlayersDbRepository(DatabaseContext dbContext) : IPlayersDbRepository
{
    private readonly DatabaseContext _dbContext = dbContext;

    public async Task CreatePlayerAsync(Player player, CancellationToken ct)
    {
        var entity = new PlayerEntity
        {
            Id = player.Id,
            Name = player.Name,
            Character = player.Character.ToString(),
            RoomId = player.RoomId,
        };

        await _dbContext.Players.AddAsync(entity, ct);

        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> HasPlayers(PlayersFilter filter, CancellationToken ct)
    {
        var query = PlayersFilterHelper.Apply(_dbContext.Players, filter);

        return await query.AnyAsync(ct);
    }
}

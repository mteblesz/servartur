using Servartur.Data.PostgreSQL.Entities;
using Servartur.Domain.DbRepositories.Filters;

namespace Servartur.Data.PostgreSQL.Repositories.FilterHelpers;

internal static class PlayersFilterHelper
{
    public static IQueryable<PlayerEntity> Apply(IQueryable<PlayerEntity> query, PlayersFilter filter)
    {
        if (filter.Names is not null && filter.Names.Any())
        {
            query = query.Where(player => filter.Names.Contains(player.Name));
        }

        if (filter.RoomId is not null)
        {
            query = query.Where(player => filter.RoomId == player.RoomId);
        }

        return query;
    }
}

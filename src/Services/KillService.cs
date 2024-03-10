using AutoMapper;
using servartur.Entities;

namespace servartur.Services;

public interface IKillService
{
    bool KillPlayer(int playerId);
}
public class KillService : BaseService, IKillService
{
    public KillService(GameDbContext dbContext, IMapper mapper, ILogger<KillService> logger)
        : base(dbContext, mapper, logger) { }

    public bool KillPlayer(int playerId)
    {
        throw new NotImplementedException();
    }
}

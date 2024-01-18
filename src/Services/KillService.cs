using AutoMapper;
using servartur.Entities;

namespace servartur.Services;

public interface IKillService
{
    bool KillPlayer(int playerId);
}
public class KillService : IKillService
{
    private readonly GameDbContext _dbContext;
    private readonly IMapper _mapper;
    public readonly ILogger<MatchupService> _logger;

    public KillService(GameDbContext dbContext, IMapper mapper, ILogger<MatchupService> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public bool KillPlayer(int playerId)
    {
        throw new NotImplementedException();
    }
}

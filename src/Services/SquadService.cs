using AutoMapper;
using servartur.Entities;

namespace servartur.Services;

public interface ISquadService
{
    void AddMember(int playerId);
    void RemoveMember(int playerId);
    void SubmitSquad(int squadId);
}
public class SquadService : ISquadService
{
    private readonly GameDbContext _dbContext;
    private readonly IMapper _mapper;
    public readonly ILogger<MatchupService> _logger;

    public SquadService(GameDbContext dbContext, IMapper mapper, ILogger<MatchupService> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    void AddMember(int playerId)
    {
        throw new NotImplementedException();
    }

    void RemoveMember(int playerId)
    {
        throw new NotImplementedException();
    }

    void SubmitSquad(int squadId)
    {
        throw new NotImplementedException();
    }
}

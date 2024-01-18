using AutoMapper;
using servartur.Entities;

namespace servartur.Services;

public interface IVoteService
{
    void VoteSquad(bool vote);
    void VoteQuest(bool vote);

}
public class VoteService : IVoteService
{
    private readonly GameDbContext _dbContext;
    private readonly IMapper _mapper;
    public readonly ILogger<MatchupService> _logger;

    public VoteService(GameDbContext dbContext, IMapper mapper, ILogger<MatchupService> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public void VoteSquad(bool vote)
    {
        throw new NotImplementedException();
    }

    public void VoteQuest(bool vote)
    {
        throw new NotImplementedException();
    }
}

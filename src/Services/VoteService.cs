using AutoMapper;
using servartur.Entities;
using servartur.Models;

namespace servartur.Services;

public interface IVoteService
{
    void VoteSquad(VoteDto voteDto);
    void VoteQuest(VoteDto voteDto);

}
public class VoteService : IVoteService
{
    private readonly GameDbContext _dbContext;
    private readonly IMapper _mapper;
    public readonly ILogger<VoteService> _logger;

    public VoteService(GameDbContext dbContext, IMapper mapper, ILogger<VoteService> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public void VoteSquad(VoteDto voteDto)
    {
        throw new NotImplementedException();
    }

    public void VoteQuest(VoteDto voteDto)
    {
        throw new NotImplementedException();
    }
}

using AutoMapper;
using servartur.Entities;
using servartur.Models;

namespace servartur.Services;

public interface IVoteService
{
    void VoteSquad(VoteDto voteDto);
    void VoteQuest(VoteDto voteDto);

}
public class VoteService : BaseService, IVoteService
{
    public VoteService(GameDbContext dbContext, IMapper mapper, ILogger<VoteService> logger)
        : base(dbContext, mapper, logger) { }

    public void VoteSquad(VoteDto voteDto)
    {
        throw new NotImplementedException();
    }

    public void VoteQuest(VoteDto voteDto)
    {
        throw new NotImplementedException();
    }
}

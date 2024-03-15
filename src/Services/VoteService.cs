using AutoMapper;
using servartur.Entities;
using servartur.Models.Incoming;

namespace servartur.Services;

public interface IVoteService
{
    void VoteSquad(CastVoteDto voteDto);
    void VoteQuest(CastVoteDto voteDto);

}
public class VoteService : BaseService, IVoteService
{
    public VoteService(GameDbContext dbContext, IMapper mapper, ILogger<VoteService> logger)
        : base(dbContext, mapper, logger) { }

    public void VoteSquad(CastVoteDto voteDto)
    {
        throw new NotImplementedException();
    }

    public void VoteQuest(CastVoteDto voteDto)
    {
        throw new NotImplementedException();
    }
}

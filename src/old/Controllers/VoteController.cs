using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using servartur.Models.Incoming;
using servartur.RealTimeUpdates;
using servartur.Services;

namespace servartur.Controllers;

[ApiController]
[Route("api/[controller]")]
internal class VoteController : ControllerBase
{
    private readonly IVoteService _voteService;
    private readonly IHubContext<UpdatesHub, IUpdatesHubClient> _hubContext;

    public VoteController(IVoteService voteService, IHubContext<UpdatesHub, IUpdatesHubClient> hubContext)
    {
        _voteService = voteService;
        _hubContext = hubContext;
    }

    [HttpPost("squad")]
    public ActionResult VoteSquad([FromBody] CastVoteDto voteDto)
    {
        _voteService.VoteSquad(voteDto, out var votingEnded, out var roomId);
        if (votingEnded)
        {
            var info = _voteService.GetUpdatedSquadVotingEnded(roomId);
            _ = _hubContext.RefreshSquadVotingEndedInfo(roomId, info);
        }
        return NoContent();
    }

    [HttpPost("quest")]
    public ActionResult VoteQuest([FromBody] CastVoteDto voteDto)
    {
        _voteService.VoteQuest(voteDto, out var votingEnded, out var roomId);
        if (votingEnded)
        {
            var info = _voteService.GetUpdatedQuestVotingEnded(roomId);
            _ = _hubContext.RefreshQuestVotingEndedInfo(roomId, info);
        }
        return NoContent();
    }
}

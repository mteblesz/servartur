using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using servartur.Entities;
using servartur.Models.Incoming;
using servartur.RealTimeUpdates;
using servartur.Services;

namespace servartur.Controllers;
[ApiController]
[Route("api/[controller]")]
public class VoteController : ControllerBase
{
    private readonly IVoteService _voteService;
    private readonly IHubContext<UpdatesHub, IUpdatesHubClient> _hubContext;

    public VoteController(IVoteService voteService, IHubContext<UpdatesHub, IUpdatesHubClient> hubContext)
    {
        this._voteService = voteService;
        this._hubContext = hubContext;
    }

    [HttpPost("squad")]
    public ActionResult VoteSquad([FromBody] CastVoteDto voteDto)
    {
        _voteService.VoteSquad(voteDto, out bool votingEnded, out int roomId);
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
        _voteService.VoteQuest(voteDto, out bool votingEnded, out int roomId);
        if (votingEnded)
        {
            var info = _voteService.GetUpdatedQuestVotingEnded(roomId);
            _ = _hubContext.RefreshQuestVotingEndedInfo(roomId, info);
        }
        return NoContent();
    }
}

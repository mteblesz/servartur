using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
            refreshData(roomId);
        }
        return NoContent();
    }

    [HttpPost("quest")]
    public ActionResult VoteQuest([FromBody] CastVoteDto voteDto)
    {
        _voteService.VoteQuest(voteDto, out bool votingEnded, out int roomId);
        if (votingEnded)
        {
            refreshData(roomId);
        }
        return NoContent();
    }

    private void refreshData(int roomId)
    {
        var curentSquad = _voteService.GetUpdatedCurrentSquad(roomId);
        _ = _hubContext.RefreshCurrentSquad(roomId, curentSquad);

        var questsSummary = _voteService.GetUpdatedQuestsSummary(roomId);
        _ = _hubContext.RefreshQuestsSummary(roomId, questsSummary);

        var endGameInfo = _voteService.GetUpdatedEndGameInfo(roomId);
        _ = _hubContext.RefreshEndGameInfo(roomId, endGameInfo);
    }
}

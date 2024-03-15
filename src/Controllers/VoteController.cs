using Microsoft.AspNetCore.Mvc;
using servartur.Models.Incoming;
using servartur.Services;

namespace servartur.Controllers;
[ApiController]
[Route("api/[controller]")]
public class VoteController : ControllerBase
{
    private readonly IVoteService _voteService;

    public VoteController(IVoteService voteService)
    {
        this._voteService = voteService;
    }

    [HttpPost("squad")]
    public ActionResult VoteSquad([FromBody] CastVoteDto voteDto)
    {
        _voteService.VoteSquad(voteDto);
        return NoContent();
    }

    [HttpPost("quest")]
    public ActionResult VoteQuest([FromBody] CastVoteDto voteDto)
    {
        _voteService.VoteSquad(voteDto);
        return NoContent();
    }
}

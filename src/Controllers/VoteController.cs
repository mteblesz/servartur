using Microsoft.AspNetCore.Mvc;
using servartur.Services;

namespace servartur.Controllers;
[Route("api/[controller]")]
[ApiController]
public class VoteController : ControllerBase
{
    private readonly IVoteService _voteService;

    public VoteController(IVoteService voteService)
    {
        this._voteService = voteService;
    }

    [HttpPost]
    public ActionResult VoteSquad(bool vote)
    {
        _voteService.VoteSquad(vote);
        return NoContent();
    }

    [HttpPost]
    public ActionResult VoteQuest(bool vote)
    {
        _voteService.VoteSquad(vote);
        return NoContent();
    }
}

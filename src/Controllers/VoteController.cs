using Microsoft.AspNetCore.Mvc;
using servartur.Models;
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

    [HttpPost]
    public ActionResult VoteSquad([FromBody] VoteDto voteDto)
    {
        _voteService.VoteSquad(voteDto);
        return NoContent();
    }

    [HttpPost]
    public ActionResult VoteQuest([FromBody] VoteDto voteDto)
    {
        _voteService.VoteSquad(voteDto);
        return NoContent();
    }
}

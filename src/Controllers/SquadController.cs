using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using servartur.Services;

namespace servartur.Controllers;
[Route("api/[controller]")]
[ApiController]
public class SquadController : ControllerBase
{
    private readonly ISquadService _squadService;

    public SquadController(ISquadService squadService)
    {
        this._squadService = squadService;
    }

    [HttpPost("add/{playerId}")]
    public ActionResult AddMember([FromRoute] int playerId)
    {
        _squadService.AddMember(playerId);
        return NoContent();
    }

    [HttpDelete("remove/{playerId}")]
    public ActionResult RemoveMember([FromRoute] int playerId)
    {
        _squadService.RemoveMember(playerId);
        return NoContent();
    }

    [HttpPatch("submit/{squadId}")]
    public ActionResult SubmitSquad([FromRoute] int squadId)
    {
        _squadService.SubmitSquad(squadId);
        return NoContent();
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using servartur.Services;

namespace servartur.Controllers;
[Route("api/[controller]")]
[ApiController]
public class KillController : ControllerBase
{
    private readonly IKillService _killService;

    public KillController(IKillService KillService)
    {
        this._killService = KillService;
    }

    [HttpPost("{playerId}")]
    public ActionResult<bool> KillPlayer(int playerId)
    {
        _killService.KillPlayer(playerId);
        return NoContent();
    }
}

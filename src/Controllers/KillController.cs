using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using servartur.Models.Incoming;
using servartur.RealTimeUpdates;
using servartur.Services;

namespace servartur.Controllers;
[ApiController]
[Route("api/[controller]")]
public class KillController : ControllerBase
{
    private readonly IKillService _killService;
    private readonly IHubContext<UpdatesHub, IUpdatesHubClient> _hubContext;

    public KillController(IKillService KillService, IHubContext<UpdatesHub, IUpdatesHubClient> hubContext)
    {
        this._killService = KillService;
        this._hubContext = hubContext;
    }

    [HttpPost]
    public ActionResult<bool> KillPlayer([FromBody] KillPlayerDto dto)
    {
        _killService.KillPlayer(dto, out int roomId);
        refreshEndGameData(roomId);
        return NoContent();
    }
    private void refreshEndGameData(int roomId)
    {
        var endGameInfo = _killService.GetUpdatedEndGame(roomId);
        _ = _hubContext.RefreshEndGameInfo(roomId, endGameInfo);
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using servartur.Models.Incoming;
using servartur.RealTimeUpdates;
using servartur.Services;

namespace servartur.Controllers;

[ApiController]
[Route("api/[controller]")]
internal class KillController : ControllerBase
{
    private readonly IKillService _killService;
    private readonly IHubContext<UpdatesHub, IUpdatesHubClient> _hubContext;

    public KillController(IKillService KillService, IHubContext<UpdatesHub, IUpdatesHubClient> hubContext)
    {
        _killService = KillService;
        _hubContext = hubContext;
    }

    [HttpPost]
    public ActionResult<bool> KillPlayer([FromBody] KillPlayerDto dto)
    {
        _killService.KillPlayer(dto, out var roomId);
        refreshEndGameData(roomId);
        return NoContent();
    }
    private void refreshEndGameData(int roomId)
    {
        var endGameInfo = _killService.GetUpdatedEndGame(roomId);
        _ = _hubContext.RefreshEndGameInfo(roomId, endGameInfo);
    }
}

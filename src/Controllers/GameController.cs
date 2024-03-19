using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using servartur.Entities;
using servartur.Enums;
using servartur.Models.Outgoing;
using servartur.RealTimeUpdates;
using servartur.Services;

namespace servartur.Controllers;
[Route("api/[controller]")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly IHubContext<UpdatesHub, IUpdatesHubClient> _hubContext;

    public GameController(IGameService gameService, IHubContext<UpdatesHub, IUpdatesHubClient> hubContext)
    {
        this._gameService = gameService;
        this._hubContext = hubContext;
    }

    [HttpDelete("leave/{playerId}")]
    public ActionResult LeaveGame([FromRoute] int playerId)
    {
        var playerInfoDto = _gameService.LeaveGame(playerId, out int roomId);
        sendPlayerLeftInfo(roomId, playerInfoDto);
        return NoContent();
    }

    [HttpGet("players/{roomId}")]
    public ActionResult<List<PlayerInfoDto>> GetPlayers([FromRoute] int roomId)
    {
        var goodPlayers = _gameService.GetUpdatedPlayers(roomId);
        return Ok(goodPlayers);
    }

    private void sendPlayerLeftInfo(int roomId, PlayerInfoDto playerInfoDto)
    {
        _ = _hubContext.SendPlayerLeftInfo(roomId, playerInfoDto);
    }
}

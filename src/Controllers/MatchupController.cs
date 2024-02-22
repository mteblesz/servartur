using Microsoft.AspNetCore.Mvc;
using servartur.Models;
using servartur.Services;
using servartur.RealTimeUpdates;
using Microsoft.AspNetCore.SignalR;
using servartur.Entities;

namespace servartur.Controllers;
[ApiController]
[Route("api/[controller]")]
public class MatchupController : ControllerBase
{
    private readonly IMatchupService _matchupService;
    private readonly IHubContext<GameHub, IGameClient> _hubContext;

    public MatchupController(IMatchupService matchupService, IHubContext<GameHub, IGameClient> hubContext)
    {
        this._matchupService = matchupService;
        this._hubContext = hubContext;
    }

    [HttpPost("room")]
    public ActionResult CreateRoom()
    {
        int roomId = _matchupService.CreateRoom();
        return Created($"/room/{roomId}", null);
    }

    [HttpPost("join/{roomId}")]
    public ActionResult JoinRoom([FromRoute] int roomId)
    {
        int playerId = _matchupService.JoinRoom(roomId);
        var players = _matchupService.GetUpdatedPlayers(roomId);

        _hubContext.Clients.All.ReceivePlayerList(players);
        return Created($"/player/{playerId}", null);
    }

    [HttpPatch("nick")]
    public ActionResult SetNickname([FromBody] PlayerNicknameSetDto dto)
    {
        _matchupService.SetNickname(dto);
        var roomId = _matchupService.GetRoomId(dto.PlayerId);
        var players = _matchupService.GetUpdatedPlayers(roomId);

        _hubContext.Clients.All.ReceivePlayerList(players);
        return NoContent();
    }

    [HttpDelete("remove/{playerId}")]
    public ActionResult RemovePlayer([FromRoute] int playerId)
    {
        var roomId = _matchupService.GetRoomId(playerId);
        _matchupService.RemovePlayer(playerId);
        var players = _matchupService.GetUpdatedPlayers(roomId);

        _hubContext.Clients.All.ReceivePlayerList(players);
        return NoContent();
    }

    [HttpPut("start")]
    public ActionResult StartGame([FromBody] StartGameDto dto)
    {
        // check game rules
        if (!(dto.AreMerlinAndAssassinInGame) && dto.ArePercivalAreMorganaInGame)
        {
            ModelState.AddModelError("", "Morgana and Percival can't be present with Merlin and Assassin missing");
            return BadRequest(ModelState);
        }
        if (!(dto.AreMerlinAndAssassinInGame) && dto.AreOberonAndMordredInGame)
        {
            ModelState.AddModelError("", "Oberon and Mordred can't be present with Merlin and Assassin missing");
            return BadRequest(ModelState);
        }
        _matchupService.StartGame(dto);

        // _hubContext.Clients.All.ReceiveRoomStartedInfo(room);
        return NoContent();
    }
}

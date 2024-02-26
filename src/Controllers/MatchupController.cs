using Microsoft.AspNetCore.Mvc;
using servartur.Models;
using servartur.Services;
using servartur.RealTimeUpdates;
using Microsoft.AspNetCore.SignalR;
using servartur.Entities;
using Microsoft.AspNetCore.SignalR.Client;

namespace servartur.Controllers;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using GameHubContext = IHubContext<GameHub, IGameHubClient>;

[ApiController]
[Route("api/[controller]")]
public class MatchupController : ControllerBase
{
    private readonly IMatchupService _matchupService;
    private readonly GameHubContext _hubContext;

    public MatchupController(IMatchupService matchupService, GameHubContext hubContext)
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
    public async Task<ActionResult> JoinRoomAsync([FromRoute] int roomId, [FromQuery] string hubConnectionId)
    {
        int playerId = _matchupService.JoinRoom(roomId);
        await _hubContext.AddToRoomGroup(roomId, hubConnectionId);

        var players = _matchupService.GetUpdatedPlayers(roomId);
        _hubContext.RefreshPlayers(roomId, players);
        return Created($"/player/{playerId}", null);
    }

    [HttpPatch("nick")]
    public ActionResult SetNickname([FromBody] PlayerNicknameSetDto dto)
    {
        _matchupService.SetNickname(dto);
        var players = _matchupService.GetUpdatedPlayers(dto.RoomId);

        _hubContext.Clients.All.ReceivePlayerList(players);
        return NoContent();
    }

    [HttpDelete("remove/{playerId}")]
    public async Task<ActionResult> RemovePlayer([FromRoute] int playerId, [FromBody] RoomConnectionDto dto)
    {
        _matchupService.RemovePlayer(playerId);
        await _hubContext.RemoveFromRoomGroup(dto.RoomId, dto.HubConnectionId);

        var players = _matchupService.GetUpdatedPlayers(dto.RoomId);
        _hubContext.RefreshPlayers(dto.RoomId, players);
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

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
    private readonly IHubFacade _hubFacade;

    public MatchupController(IMatchupService matchupService, IHubFacade hubFacade)
    {
        this._matchupService = matchupService;
        this._hubFacade = hubFacade;
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
        await _hubFacade.AddToRoomGroup(roomId, hubConnectionId);

        var players = _matchupService.GetUpdatedPlayers(roomId);
        await _hubFacade.RefreshPlayers(roomId, players);
        return Created($"/player/{playerId}", null);
    }

    [HttpPatch("nick")]
    public async Task<ActionResult> SetNicknameAsync([FromBody] PlayerNicknameSetDto dto)
    {
        _matchupService.SetNickname(dto);
        var players = _matchupService.GetUpdatedPlayers(dto.RoomId);

        await _hubFacade.RefreshPlayers(dto.RoomId, players);
        return NoContent();
    }

    [HttpDelete("remove/{playerId}")]
    public async Task<ActionResult> RemovePlayerAsync([FromRoute] int playerId, [FromBody] RoomConnectionDto dto)
    {
        _matchupService.RemovePlayer(playerId);
        await _hubFacade.RemoveFromRoomGroup(dto.RoomId, dto.HubConnectionId);

        await _hubFacade.SendRemovalInfo(playerId);

        var players = _matchupService.GetUpdatedPlayers(dto.RoomId);
        await _hubFacade.RefreshPlayers(dto.RoomId, players);
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

        // _hubFacade.Clients.All.ReceiveRoomStartedInfo(room);
        return NoContent();
    }
}

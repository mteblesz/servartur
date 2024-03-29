﻿using Microsoft.AspNetCore.Mvc;
using servartur.Services;
using servartur.RealTimeUpdates;
using Microsoft.AspNetCore.SignalR;
using servartur.Models.Incoming;
using servartur.Utils;

namespace servartur.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchupController : ControllerBase
{
    private readonly IMatchupService _matchupService;
    private readonly IHubContext<UpdatesHub, IUpdatesHubClient> _hubContext;

    public MatchupController(IMatchupService matchupService, IHubContext<UpdatesHub, IUpdatesHubClient> hubContext)
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
        refreshPlayersData(roomId);
        return Created($"/player/{playerId}", null);
    }

    [HttpPatch("nick")]
    public ActionResult SetNickname([FromBody] PlayerNicknameSetDto dto)
    {
        _matchupService.SetNickname(dto);
        refreshPlayersData(dto.RoomId);
        return NoContent();
    }

    [HttpDelete("remove/{playerId}/from/{roomId}")]
    public ActionResult RemovePlayer([FromRoute] int playerId, [FromRoute] int roomId)
    {
        _matchupService.RemovePlayer(playerId);
        refreshPlayersData(roomId);
        sendRemovalInfo(roomId, playerId);
        return NoContent();
    }

    [HttpPut("start")]
    public ActionResult StartGame([FromBody] StartGameDto dto)
    {
        // check game rules
        if (!(dto.AreMerlinAndAssassinInGame) && dto.ArePercivalAndMorganaInGame)
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

        sendStartGame(dto.RoomId);
        refreshPlayersData(dto.RoomId);
        refreshAllSquadsData(dto.RoomId);
        return NoContent();
    }

    private void refreshPlayersData(int roomId)
    {
        var players = _matchupService.GetUpdatedPlayers(roomId);
        _ = _hubContext.RefreshPlayers(roomId, players);
    }
    private void sendRemovalInfo(int roomId, int playerId)
    {
        _ = _hubContext.SendRemovalInfo(roomId, playerId);
    }
    private void sendStartGame(int roomId)
    {
        _ = _hubContext.SendStartGame(roomId);
    }
    private void refreshAllSquadsData(int roomId)
    {
        var curentSquad = _matchupService.GetUpdatedCurrentSquad(roomId);
        var questsSummary = _matchupService.GetUpdatedQuestsSummary(roomId);
        _ = _hubContext.RefreshCurrentSquad(roomId, curentSquad);
        _ = _hubContext.RefreshSquadsSummary(roomId, questsSummary);
    }
}

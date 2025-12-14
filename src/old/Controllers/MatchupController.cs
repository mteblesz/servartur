using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using servartur.Models.Incoming;
using servartur.Models.Outgoing;
using servartur.RealTimeUpdates;
using servartur.Services;

namespace servartur.Controllers;

[ApiController]
[Route("api/[controller]")]
internal class MatchupController : ControllerBase
{
    private readonly IMatchupService _matchupService;
    private readonly IHubContext<UpdatesHub, IUpdatesHubClient> _hubContext;

    public MatchupController(IMatchupService matchupService, IHubContext<UpdatesHub, IUpdatesHubClient> hubContext)
    {
        _matchupService = matchupService;
        _hubContext = hubContext;
    }

    [HttpPost("room")]
    public ActionResult CreateRoom()
    {
        var roomId = _matchupService.CreateRoom();
        return Created($"/room/{roomId}", null);
    }

    [HttpPost("join/{roomId}")]
    public ActionResult JoinRoom([FromRoute] int roomId)
    {
        var playerId = _matchupService.JoinRoom(roomId);
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
        if (!dto.AreMerlinAndAssassinInGame && dto.ArePercivalAndMorganaInGame)
        {
            ModelState.AddModelError("", "Morgana and Percival can't be present with Merlin and Assassin missing");
            return BadRequest(ModelState);
        }
        if (!dto.AreMerlinAndAssassinInGame && dto.AreOberonAndMordredInGame)
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

    [HttpDelete("leave/{playerId}")]
    public ActionResult LeaveGame([FromRoute] int playerId)
    {
        var playerInfoDto = _matchupService.LeaveGame(playerId, out var roomId);
        sendPlayerLeftInfo(roomId, playerInfoDto);
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
        _ = _hubContext.RefreshQuestsSummary(roomId, questsSummary);
    }
    private void sendPlayerLeftInfo(int roomId, PlayerInfoDto playerInfoDto)
    {
        _ = _hubContext.SendPlayerLeftInfo(roomId, playerInfoDto);
    }
}

using Microsoft.AspNetCore.Mvc;
using servartur.Entities;
using servartur.Enums;
using servartur.Models;
using servartur.Services;

namespace servartur.Controllers;
[ApiController]
[Route("api/[controller]")]
public class InfoController : ControllerBase
{
    private readonly IInfoService _infoService;

    public InfoController(IInfoService InfoService)
    {
        this._infoService = InfoService;
    }

    [HttpGet("room/{roomId}")]
    public ActionResult<RoomInfoDto> GetRoomById([FromRoute] int roomId)
    {
        var room = _infoService.GetRoomById(roomId);
        return Ok(room);
    }

    [HttpGet("player/{playerId}")]
    public ActionResult<PlayerInfoDto> GetPlayerById([FromRoute] int playerId)
    {
        var player = _infoService.GetPlayerById(playerId);
        return Ok(player);
    }

    [HttpGet("player/role{playerId}")]
    public ActionResult<PlayerRoleInfoDto> GetRoleByPlayerId([FromRoute] int playerId)
    {
        var roleInfo = _infoService.GetRoleByPlayerId(playerId);
        return Ok(roleInfo);
    }

    [HttpGet("goodplayers/{roomId}")]
    public ActionResult<List<PlayerInfoDto>> GetGoodPlayers([FromRoute] int roomId)
    {
        Predicate<Player> goodPredicate = p => p.Team == Team.Good;
        var goodPlayers = _infoService.GetFilteredPlayers(roomId, goodPredicate);
        return Ok(goodPlayers);
    }

    [HttpGet("evilplayers/{roomId}")]
    public ActionResult<List<PlayerInfoDto>> GetEvilPlayers([FromRoute] int roomId)
    {
        Predicate<Player> evilPredicate = p => p.Team == Team.Evil;
        var evilPlayers = _infoService.GetFilteredPlayers(roomId, evilPredicate);
        return Ok(evilPlayers);
    }

    [HttpGet("percival_known/{roomId}")]
    public ActionResult<List<PlayerInfoDto>> GetKnownByPercivalPlayers([FromRoute] int roomId)
    {
        var mmPlayers = _infoService.GetKnownByPercivalPlayers(roomId);
        return Ok(mmPlayers);
    }

    [HttpGet("quest/{squadId}")]
    public ActionResult<SquadInfoDto> GetQuestBySquadId([FromRoute] int squadId)
    {
        var squad = _infoService.GetSquadById(squadId);
        return Ok(squad);
    }
}

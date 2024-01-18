using Microsoft.AspNetCore.Mvc;
using servartur.Models;
using servartur.Services;

namespace servartur.Controllers;
[ApiController]
[Route("api/[controller]")]
public class MatchupController : ControllerBase
{
    private readonly IMatchupService _matchupService;

    public MatchupController(IMatchupService matchupService)
    {
        this._matchupService = matchupService;
    }

    [HttpGet("room/{roomId}")]
    public ActionResult<RoomDto> GetRoomById([FromRoute] int roomId)
    {
        var room = _matchupService.GetRoomById(roomId);
        return Ok(room);
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
        return Created($"/player/{playerId}", null);
    }

    [HttpPut("nick")]
    public ActionResult SetNickname([FromBody] PlayerNicknameSetDto dto)
    {
        _matchupService.SetNickname(dto);
        return NoContent();
    }

    [HttpDelete("remove/{playerId}")]
    public ActionResult RemovePlayer([FromRoute] int playerId)
    {
        _matchupService.RemovePlayer(playerId);
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
        return NoContent();
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using servartur.Entities;
using servartur.Exceptions;
using servartur.Models;
using servartur.Services;

namespace servartur.Controllers;
[ApiController]
[Route("/")]
public class MatchupController : ControllerBase
{
    private readonly IMatchupService _matchupService;

    public MatchupController(IMatchupService matchupService)
    {
        this._matchupService = matchupService;
    }

    [HttpPost("room")]
    public ActionResult CreateRoom()
    {
        var roomId = _matchupService.CreateRoom();
        return Created($"/api/rooms/{roomId}", null);
    }

    [HttpPost("player")]
    public ActionResult CreatePlayer([FromBody] CreatePlayerDto dto)
    {
        var playerId = _matchupService.CreatePlayer(dto);
        return Created($"/api/rooms/player/{playerId}", null);
    }

    [HttpDelete("player/{playerId}")]
    public ActionResult RemovePlayer([FromRoute] int playerId)
    {
        _matchupService.RemovePlayer(playerId);
        return NoContent();
    }

    [HttpGet("{roomId}")]
    public ActionResult<RoomDto> GetRoomById([FromRoute] int roomId)
    {
        var room = _matchupService.GetRoomById(roomId);
        return Ok(room);
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

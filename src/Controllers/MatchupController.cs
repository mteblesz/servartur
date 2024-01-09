﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using servartur.Entities;
using servartur.Exceptions;
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

    [HttpPost]
    public ActionResult CreateRoom([FromBody] CreateRoomDto dto)
    {
        var roomId = _matchupService.CreateRoom(dto);
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

    // TODO add startRoom - assign roles, create first squad and assign leader
    // TODO remove this later in favour of startRoom()
    [HttpPut("makeTeams/{roomId}")]
    public ActionResult MakeTeams([FromRoute] int roomId)
    {    
        _matchupService.MakeTeams(roomId);
        return NoContent();
    }
}

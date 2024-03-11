using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using servartur.Entities;
using servartur.Models;
using servartur.RealTimeUpdates;
using servartur.Services;

namespace servartur.Controllers;
[ApiController]
[Route("api/[controller]")]
public class SquadController : ControllerBase
{
    private readonly ISquadService _squadService;
    private readonly IHubContext<GameHub, IGameHubClient> _hubContext;

    public SquadController(ISquadService squadService, IHubContext<GameHub, IGameHubClient> hubContext)
    {
        this._squadService = squadService;
        this._hubContext = hubContext;
    }

    [HttpPost("add/{playerId}")]
    public ActionResult AddMember([FromRoute] int playerId)
    {
        _squadService.AddMember(playerId);
        // refreshSquadsData(roomId);
        return NoContent();
    }

    [HttpDelete("remove/{playerId}")]
    public ActionResult RemoveMember([FromRoute] int playerId)
    {
        _squadService.RemoveMember(playerId);
        // refreshSquadsData(roomId);
        return NoContent();
    }

    [HttpPatch("submit/{squadId}")]
    public ActionResult SubmitSquad([FromRoute] int squadId)
    {
        _squadService.SubmitSquad(squadId);
        // refreshSquadsData(roomId);
        return NoContent();
    }

    private void refreshSquadsData(int roomId)
    {
        var squads = _squadService.GetUpdatedSquads(roomId);
        _ = _hubContext.RefreshSquads(roomId, squads);
    }
}

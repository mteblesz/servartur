using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using servartur.RealTimeUpdates;
using servartur.Services;

namespace servartur.Controllers;

[ApiController]
[Route("api/[controller]")]
internal class SquadController : ControllerBase
{
    private readonly ISquadService _squadService;
    private readonly IHubContext<UpdatesHub, IUpdatesHubClient> _hubContext;

    public SquadController(ISquadService squadService, IHubContext<UpdatesHub, IUpdatesHubClient> hubContext)
    {
        _squadService = squadService;
        _hubContext = hubContext;
    }

    [HttpPost("add/{playerId}")]
    public ActionResult AddMember([FromRoute] int playerId)
    {
        _squadService.AddMember(playerId, out var roomId);
        refreshCurrentSquadsData(roomId);
        return NoContent();
    }

    [HttpDelete("remove/{playerId}")]
    public ActionResult RemoveMember([FromRoute] int playerId)
    {
        _squadService.RemoveMember(playerId, out var roomId);
        refreshCurrentSquadsData(roomId);
        return NoContent();
    }

    [HttpPatch("submit/{squadId}")]
    public ActionResult SubmitSquad([FromRoute] int squadId)
    {
        _squadService.SubmitSquad(squadId, out var roomId);
        refreshCurrentSquadsData(roomId);
        return NoContent();
    }

    private void refreshCurrentSquadsData(int roomId)
    {
        var curentSquad = _squadService.GetUpdatedCurrentSquad(roomId);
        _ = _hubContext.RefreshCurrentSquad(roomId, curentSquad);
    }
    //private void refreshQuestsSummaryData(int roomId)
    //{
    //    var questsSummary = _squadService.GetUpdatedQuestsSummary(roomId);
    //    _ = _hubContext.RefreshSquadsSummary(roomId, questsSummary);
    //}
}

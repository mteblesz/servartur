using Microsoft.AspNetCore.Mvc;
using servartur.Models;
using servartur.Services;

namespace servartur.Controllers;
[Route("api/[controller]")]
[ApiController]
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
}

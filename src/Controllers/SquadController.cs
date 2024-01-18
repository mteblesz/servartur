using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using servartur.Services;

namespace servartur.Controllers;
[Route("api/[controller]")]
[ApiController]
public class SquadController : ControllerBase
{
    private readonly ISquadService _squadService;

    public SquadController(ISquadService squadService)
    {
        this._squadService = squadService;
    }
}

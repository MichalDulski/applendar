using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Applendar.API.V2.Controllers;

[ApiController]
[ApiVersion(2.0)]
[Route("api/healthcheck")]
public class Healthcheck2Controller : ControllerBase
{
    private readonly ILogger<Healthcheck2Controller> _logger;

    public Healthcheck2Controller(ILogger<Healthcheck2Controller> logger) { _logger = logger; }

    [HttpGet]
    public ActionResult<string> Get()
    {
        return Ok("stara cichego");
    }
}
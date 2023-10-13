using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Applendar.API.V1.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/healthcheck")]
public class HealthcheckController : ControllerBase
{
    private readonly ILogger<HealthcheckController> _logger;

    public HealthcheckController(ILogger<HealthcheckController> logger) { _logger = logger; }

    [HttpGet]
    public ActionResult<string> Get()
    {
        return Ok("healthy");
    }
}
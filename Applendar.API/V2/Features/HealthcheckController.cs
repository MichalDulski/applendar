using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Applendar.API.V2.Features;

[ApiController]
[ApiVersion(2.0)]
[Route("api/healthcheck")]
public class HealthcheckController : ControllerBase
{
    private readonly ILogger<HealthcheckController> _logger;

    public HealthcheckController(ILogger<HealthcheckController> logger)
        => _logger = logger;

    [HttpGet]
    public ActionResult<string> Get()
    {
        throw new UnauthorizedAccessException();
        return Ok("healthy");
    }
}
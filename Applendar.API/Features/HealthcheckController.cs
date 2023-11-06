using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Applendar.API.Features;

[ApiController]
[ApiVersion(1.0)]
[Route("api/healthcheck")]
public class HealthcheckController : ControllerBase
{
    private readonly ILogger<HealthcheckController> _logger;

    public HealthcheckController(ILogger<HealthcheckController> logger)
        => _logger = logger;

    [HttpGet]
    public ActionResult<string> Get()
        => Ok("healthy");
}


[ApiController]
[ApiVersion(2.0)]
[Route("api/healthcheck")]
public class HealthcheckV2Controller : ControllerBase
{
    private readonly ILogger<HealthcheckV2Controller> _logger;

    public HealthcheckV2Controller(ILogger<HealthcheckV2Controller> logger)
        => _logger = logger;

    [HttpGet]
    public ActionResult<string> Get()
        => Ok("healthy");
}
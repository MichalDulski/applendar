using System.Security.Claims;
using Applander.Domain.Entities;
using Applander.Infrastructure;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Applendar.API.V1.Features.Users;

[ApiController]
[ApiVersion(1.0)]
[Route("api/account")]
[Authorize]
public class GetLoggedUserDataController : ControllerBase
{
    private readonly ILogger<GetLoggedUserDataController> _logger;
    private readonly IGetLoggedUserDataRepository _repository;

    public GetLoggedUserDataController(IGetLoggedUserDataRepository repository,
        ILogger<GetLoggedUserDataController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet()]
    public async Task<ActionResult<GetLoggedUserDataResponse>> Get()
    {
        _logger.LogInformation("Get logged user data");
        var externalUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        ApplendarUser? applendarUser = await _repository.GetUserAsync(externalUserId);

        if (applendarUser is null)
            return BadRequest("User not found");

        var response = new GetLoggedUserDataResponse(applendarUser.Id, applendarUser.FirstName,
            applendarUser.LastName, applendarUser.ExternalId, applendarUser.Preferences);

        return Ok(response);
    }
}

public record GetLoggedUserDataResponse(Guid Id,
    string FirstName,
    string LastName,
    string ExternalId,
    Preferences Preferences);

public interface IGetLoggedUserDataRepository
{
    Task<ApplendarUser?> GetUserAsync(string externalUserId, CancellationToken cancellationToken = default);
}

public class GetLoggedUserDataRepository : IGetLoggedUserDataRepository
{
    private readonly ApplendarDbContext _dbContext;

    public GetLoggedUserDataRepository(ApplendarDbContext dbContext)
        => _dbContext = dbContext;

    public async Task<ApplendarUser?> GetUserAsync(string externalUserId, CancellationToken cancellationToken = default)
        => await _dbContext.ApplendarUsers.FirstOrDefaultAsync(x => x.ExternalId == externalUserId, cancellationToken);
}
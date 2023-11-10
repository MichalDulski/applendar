using Applander.Domain.Entities;
using Applander.Infrastructure;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Applendar.API.Features.Users.V1;

[ApiController]
[ApiVersion(1.0)]
[Route("api/users")]
[Authorize]
public class GetApplendarUserProfileController : ControllerBase
{
    private readonly ILogger<GetApplendarUserProfileController> _logger;
    private readonly IGetApplendarUserProfileRepository _repository;

    public GetApplendarUserProfileController(IGetApplendarUserProfileRepository repository,
        ILogger<GetApplendarUserProfileController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet("{applendarUserId}")]
    public async Task<ActionResult<ApplendarUserProfile>> Get([FromRoute] Guid applendarUserId)
    {
        _logger.LogDebug("Invoked Get Applander user profile");

        ApplendarUser? applendarUser = await _repository.GetUserAsync(applendarUserId);

        if (applendarUser is null)
            return BadRequest("User not found");

        var response = new ApplendarUserProfile(applendarUser.Id, applendarUser.FirstName,
            applendarUser.LastName, applendarUser.ExternalId, applendarUser.Preferences);

        return Ok(response);
    }
}

public record ApplendarUserProfile(Guid Id,
    string FirstName,
    string LastName,
    string ExternalId,
    Preferences Preferences);

public interface IGetApplendarUserProfileRepository
{
    Task<ApplendarUser?> GetUserAsync(Guid applendarUserId, CancellationToken cancellationToken = default);
}

public class GetApplendarUserProfileRepository : IGetApplendarUserProfileRepository
{
    private readonly ApplendarDbContext _dbContext;

    public GetApplendarUserProfileRepository(ApplendarDbContext dbContext)
        => _dbContext = dbContext;

    public async Task<ApplendarUser?> GetUserAsync(Guid applendarUserId, CancellationToken cancellationToken = default)
        => await _dbContext.ApplendarUsers.FirstOrDefaultAsync(x => x.Id == applendarUserId, cancellationToken);
}
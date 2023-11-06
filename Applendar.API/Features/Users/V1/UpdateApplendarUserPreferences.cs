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
public class UpdateApplendarUserPreferencesController : ControllerBase
{
    private readonly ILogger<UpdateApplendarUserPreferencesController> _logger;
    private readonly IUpdateApplendarUserPreferencesRepository _repository;

    public UpdateApplendarUserPreferencesController(IUpdateApplendarUserPreferencesRepository repository,
        ILogger<UpdateApplendarUserPreferencesController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpPut("{applendarUserId}/preferences")]
    public async Task<ActionResult<UpdateApplendarUserPreferencesResponse>> Put([FromRoute] Guid applendarUserId,
        [FromBody] Preferences request)
    {
        _logger.LogInformation("Update Applander user preferences");

        ApplendarUser? applendarUser = await _repository.GetUserAsync(applendarUserId);

        if (applendarUser is null)
            return BadRequest("User not found");

        applendarUser.UpdateUserPreferences(request);
        await _repository.SaveChangesAsync();

        var response = new UpdateApplendarUserPreferencesResponse(applendarUser.Id, applendarUser.FirstName,
            applendarUser.LastName, applendarUser.ExternalId, applendarUser.Preferences);

        return Ok(response);
    }
}

public record UpdateApplendarUserPreferencesResponse(Guid Id,
    string FirstName,
    string LastName,
    string ExternalId,
    Preferences Preferences);

public interface IUpdateApplendarUserPreferencesRepository
{
    Task<ApplendarUser?> GetUserAsync(Guid applendarUserId, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class UpdateApplendarUserPreferencesRepository : IUpdateApplendarUserPreferencesRepository
{
    private readonly ApplendarDbContext _dbContext;

    public UpdateApplendarUserPreferencesRepository(ApplendarDbContext dbContext)
        => _dbContext = dbContext;

    public async Task<ApplendarUser?> GetUserAsync(Guid applendarUserId, CancellationToken cancellationToken = default)
        => await _dbContext.ApplendarUsers.FirstOrDefaultAsync(x => x.Id == applendarUserId, cancellationToken);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);
}
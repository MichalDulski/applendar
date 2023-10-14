using Applander.Domain.Entities;
using Applander.Infrastructure;
using Applendar.API.V1.Features.Events;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Applendar.API.V1.Features.Users;

[ApiController]
[ApiVersion(1.0)]
[Route("api/users")]
public class UpdateApplendarUserPreferencesController : ControllerBase
{
    private readonly IUpdateApplendarUserPreferencesRepository _repository;
    private readonly ILogger<UpdateApplendarUserPreferencesController> _logger;

    public UpdateApplendarUserPreferencesController(IUpdateApplendarUserPreferencesRepository repository, ILogger<UpdateApplendarUserPreferencesController> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    [HttpPut("{applendarUserId}/preferences")]
    public async Task<ActionResult<UpdateApplendarUserPreferencesResponse>> Put([FromRoute] Guid applendarUserId, [FromBody] Preferences request)
    {
        _logger.LogInformation("Update Applander user preferences");
        
        var applendarUser = await _repository.GetUserAsync(applendarUserId);
        
        if(applendarUser is null)
        {
            return BadRequest("User not found");
        }
        
        applendarUser.UpdateUserPreferences(request);
        await _repository.SaveChangesAsync();
    
        var response = new UpdateApplendarUserPreferencesResponse(applendarUser.Id, applendarUser.FirstName, applendarUser.LastName, applendarUser.ExternalId, applendarUser.Preferences);
    
        return Ok(response);
    }
}

public record UpdateApplendarUserPreferencesResponse(Guid Id, string FirstName, string LastName, string ExternalId, Preferences Preferences);

public interface IUpdateApplendarUserPreferencesRepository
{
    Task<ApplendarUser?> GetUserAsync(Guid applendarUserId, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class UpdateApplendarUserPreferencesRepository : IUpdateApplendarUserPreferencesRepository
{
    private readonly ApplanderDbContext _dbContext;

    public UpdateApplendarUserPreferencesRepository(ApplanderDbContext dbContext) { _dbContext = dbContext; }

    public async Task<ApplendarUser?> GetUserAsync(Guid applendarUserId, CancellationToken cancellationToken = default)
        => await _dbContext.ApplendarUsers.FirstOrDefaultAsync(x => x.Id == applendarUserId, cancellationToken);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);
}
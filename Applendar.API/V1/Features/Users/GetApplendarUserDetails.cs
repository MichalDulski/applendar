using Applander.Domain.Entities;
using Applander.Infrastructure;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Applendar.API.V1.Features.Users;

[ApiController]
[ApiVersion(1.0)]
[Route("api/users")]
public class GetApplendarUserDetailsController : ControllerBase
{
    private readonly ILogger<GetApplendarUserDetailsController> _logger;
    private readonly IGetApplendarUserDetailsRepository _repository;

    public GetApplendarUserDetailsController(IGetApplendarUserDetailsRepository repository,
        ILogger<GetApplendarUserDetailsController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet("{applendarUserId}")]
    public async Task<ActionResult<GetApplendarUserDetailsResponse>> Get([FromRoute] Guid applendarUserId)
    {
        _logger.LogInformation("Get Applander user details");

        ApplendarUser? applendarUser = await _repository.GetUserAsync(applendarUserId);

        if (applendarUser is null)
            return BadRequest("User not found");

        var response = new GetApplendarUserDetailsResponse(applendarUser.Id, applendarUser.FirstName,
            applendarUser.LastName, applendarUser.ExternalId, applendarUser.Preferences);

        return Ok(response);
    }
}

public record GetApplendarUserDetailsResponse(Guid Id,
    string FirstName,
    string LastName,
    string ExternalId,
    Preferences Preferences);

public interface IGetApplendarUserDetailsRepository
{
    Task<ApplendarUser?> GetUserAsync(Guid applendarUserId, CancellationToken cancellationToken = default);
}

public class GetApplendarUserDetailsRepository : IGetApplendarUserDetailsRepository
{
    private readonly ApplanderDbContext _dbContext;

    public GetApplendarUserDetailsRepository(ApplanderDbContext dbContext)
        => _dbContext = dbContext;

    public async Task<ApplendarUser?> GetUserAsync(Guid applendarUserId, CancellationToken cancellationToken = default)
        => await _dbContext.ApplendarUsers.FirstOrDefaultAsync(x => x.Id == applendarUserId, cancellationToken);
}
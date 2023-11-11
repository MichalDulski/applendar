using System.Security.Claims;
using Applander.Domain.Entities;
using Applander.Infrastructure;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Applendar.API.Features.Users.V1.RegisterApplendarUser;

[ApiController]
[ApiVersion(1.0)]
[Route("api/users")]
[Authorize]
public class RegisterApplendarUserController : ControllerBase
{
    private readonly ILogger<RegisterApplendarUserController> _logger;
    private readonly IRegisterApplendarUserRepository _repository;

    public RegisterApplendarUserController(IRegisterApplendarUserRepository repository,
        ILogger<RegisterApplendarUserController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<RegisterUserResponse>> Post([FromBody] RegisterUserRequest request)
    {
        _logger.LogInformation("Adding Applander user");

        var user = ApplendarUser.Create(request.FirstName, request.LastName, request.ExternalId);

        string? externalUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (externalUserId is null)
        {
            return BadRequest("There was a problem with your token");
        }
        
        _repository.AddUser(user);
        await _repository.SaveChangesAsync();

        var response = new RegisterUserResponse(user.Id, user.FirstName, user.LastName,
            user.ExternalId);

        return Ok(response);
    }
}

public record RegisterUserRequest(string FirstName,
    string LastName,
    string ExternalId);

public record RegisterUserResponse(Guid Id,
    string FirstName,
    string LastName,
    string ExternalId);

public interface IRegisterApplendarUserRepository
{
    void AddUser(ApplendarUser applendarUser);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

internal class RegisterApplendarUserRepository : IRegisterApplendarUserRepository
{
    private readonly ApplendarDbContext _dbContext;

    public RegisterApplendarUserRepository(ApplendarDbContext dbContext)
        => _dbContext = dbContext;

    public void AddUser(ApplendarUser applendarUser)
        => _dbContext.ApplendarUsers.Add(applendarUser);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);
}
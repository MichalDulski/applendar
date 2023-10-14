using Applander.Domain.Entities;
using Applander.Infrastructure;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Applendar.API.V1.Features.Users;

[ApiController]
[ApiVersion(1.0)]
[Route("api/users")]
public class RegisterApplendarUserController : ControllerBase
{
    private readonly ILogger<RegisterApplendarUserController> _logger;
    private readonly IRegisterApplendarUserRepository _registerApplendarUserRepository;

    public RegisterApplendarUserController(IRegisterApplendarUserRepository registerApplendarUserRepository,
        ILogger<RegisterApplendarUserController> logger)
    {
        _registerApplendarUserRepository = registerApplendarUserRepository;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<RegisterUserResponse>> Post([FromBody] RegisterUserRequest request)
    {
        _logger.LogInformation("Adding Applander user");

        var user = ApplendarUser.Create(request.FirstName, request.LastName, request.ExternalId);

        _registerApplendarUserRepository.AddUser(user);
        await _registerApplendarUserRepository.SaveChangesAsync();

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

public class RegisterApplendarUserRepository : IRegisterApplendarUserRepository
{
    private readonly ApplanderDbContext _dbContext;

    public RegisterApplendarUserRepository(ApplanderDbContext dbContext)
        => _dbContext = dbContext;

    public void AddUser(ApplendarUser applendarUser)
        => _dbContext.ApplendarUsers.Add(applendarUser);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);
}
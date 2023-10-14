using Applander.Domain.Common;
using Applander.Domain.Entities;
using Applander.Infrastructure;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Applendar.API.V1.Features.Users;

[ApiController]
[ApiVersion(1.0)]
[Route("api/users")]
public class GetUserEventInvitationsController : ControllerBase
{
    private readonly IGetUserEventInvitationsRepository _repository;
    private readonly ILogger<GetUserEventInvitationsController> _logger;

    public GetUserEventInvitationsController(IGetUserEventInvitationsRepository repository, ILogger<GetUserEventInvitationsController> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    [HttpGet("{applendarUserId}/invitations")]
    public async Task<ActionResult<GetUserEventInvitationDto[]>> Get([FromRoute] Guid applendarUserId)
    {
        _logger.LogInformation("Get Applander user details");
        
        var applendarUserInvitations = await _repository.GetUserInvitationsAsync(applendarUserId);

        var invitationDtos = applendarUserInvitations.Select(x => new GetUserEventInvitationDto(x.Event.Name, x.Event.StartAtUtc, x.Event.Location, x.Event.EventType, x.Event.OrganizerId, x.Status, x.Event.MaximumNumberOfParticipants, x.Event.IsCompanionAllowed, x.Event.IsPetAllowed)).ToArray();
    
        return Ok(invitationDtos);
    }
}

public record GetUserEventInvitationDto(string Name,
    DateTime StartAtUtc,
    Location Location,
    EventType EventType,
    Guid OrganizerId,
    InvitationStatus Status,
    int? MaximumNumberOfParticipants = null,
    bool IsCompanionAllowed = false,
    bool IsPetAllowed = false);

public interface IGetUserEventInvitationsRepository
{
    Task<ICollection<EventInvitation>> GetUserInvitationsAsync(Guid applendarUserId, CancellationToken cancellationToken = default);
}

public class GetUserEventInvitationsRepository : IGetUserEventInvitationsRepository
{
    private readonly ApplanderDbContext _dbContext;

    public GetUserEventInvitationsRepository(ApplanderDbContext dbContext) { _dbContext = dbContext; }

    public async Task<ICollection<EventInvitation>> GetUserInvitationsAsync(Guid applendarUserId,
        CancellationToken cancellationToken = default)
        => await _dbContext.EventInvitations.AsNoTracking()
            .Include(x => x.Event)
            .Where(x => x.ApplendarUserId == applendarUserId)
            .ToListAsync(cancellationToken);
}
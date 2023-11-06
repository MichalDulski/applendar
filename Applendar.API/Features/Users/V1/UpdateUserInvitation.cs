using Applander.Domain.Common;
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
public class UpdateUserInvitationController : ControllerBase
{
    private readonly ILogger<UpdateUserInvitationController> _logger;
    private readonly IUpdateUserInvitationRepository _repository;

    public UpdateUserInvitationController(IUpdateUserInvitationRepository repository,
        ILogger<UpdateUserInvitationController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpPut("{applendarUserId}/invitations/{eventId}")]
    public async Task<ActionResult<UpdateUserInvitationResponse>> Put([FromRoute] Guid applendarUserId,
        [FromRoute] Guid eventId,
        [FromBody] UpdateUserInvitationsRequest request)
    {
        _logger.LogInformation("Update Applander user event invitation");

        EventInvitation? eventInvitation = await _repository.GetUserInvitationAsync(applendarUserId, eventId);

        if (eventInvitation is null)
            return BadRequest("Invitation not found");

        eventInvitation.ChangeInvitationStatus(request.Status);
        await _repository.SaveChangesAsync();

        var response = new UpdateUserInvitationResponse(eventInvitation.ApplendarUserId, eventInvitation.EventId,
            eventInvitation.Status);

        return Ok(response);
    }
}

public record UpdateUserInvitationsRequest(InvitationStatus Status);

public record UpdateUserInvitationResponse(Guid ApplendarUserId,
    Guid EventId,
    InvitationStatus Status);

public interface IUpdateUserInvitationRepository
{
    Task<EventInvitation?> GetUserInvitationAsync(Guid applendarUserId,
        Guid eventId,
        CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class UpdateUserInvitationRepository : IUpdateUserInvitationRepository
{
    private readonly ApplendarDbContext _dbContext;

    public UpdateUserInvitationRepository(ApplendarDbContext dbContext)
        => _dbContext = dbContext;

    public async Task<EventInvitation?> GetUserInvitationAsync(Guid applendarUserId,
        Guid eventId,
        CancellationToken cancellationToken = default)
        => await _dbContext.EventInvitations.FirstOrDefaultAsync(
            x => x.ApplendarUserId == applendarUserId && x.EventId == eventId, cancellationToken);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);
}
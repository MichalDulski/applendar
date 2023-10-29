using Applander.Domain.Common;
using Applander.Domain.Entities;
using Applander.Infrastructure;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Applendar.API.V1.Features.Events;

[ApiController]
[ApiVersion(1.0)]
[Route("api/events")]
[Authorize]
public class GetEventDetailsController : ControllerBase
{
    private readonly IGetEventDetailsRepository _getEventDetailsRepository;
    private readonly ILogger<GetEventDetailsController> _logger;

    public GetEventDetailsController(ILogger<GetEventDetailsController> logger,
        IGetEventDetailsRepository getEventDetailsRepository)
    {
        _logger = logger;
        _getEventDetailsRepository = getEventDetailsRepository;
    }

    [HttpGet("{eventId}")]
    public async Task<ActionResult<GetEventDetailsResult>> Get([FromRoute] Guid eventId)
    {
        _logger.LogInformation("Get event details");
        Event? eventDetails = await _getEventDetailsRepository.GetEventDetailsAsync(eventId);

        if (eventDetails is null)
            return BadRequest("Not found");

        var invitedUsers = eventDetails.Invitations.Select(x
                => new InvitedUser(x.ApplendarUser.Id, x.ApplendarUser.FirstName, x.ApplendarUser.LastName,
                    x.Status))
            .ToList();

        var eventDetailsDto = new GetEventDetailsResult(eventDetails.Id, eventDetails.Name, eventDetails.StartAtUtc,
            eventDetails.Location, eventDetails.EventType, eventDetails.OrganizerId,
            invitedUsers, eventDetails.MaximumNumberOfParticipants, eventDetails.IsCompanionAllowed,
            eventDetails.IsPetAllowed, eventDetails.Image != null ? Convert.ToBase64String(eventDetails.Image) : null);

        return Ok(eventDetailsDto);
    }
}

public record GetEventDetailsResult(Guid Id,
    string Name,
    DateTime StartAtUtc,
    Location Location,
    EventType EventType,
    Guid OrganizerId,
    ICollection<InvitedUser> InvitedUsers,
    int? MaximumNumberOfParticipants = null,
    bool IsCompanionAllowed = false,
    bool IsPetAllowed = false,
    string? Base64Image = null);

public record InvitedUser(Guid Id,
    string FirstName,
    string LastName,
    InvitationStatus Status);

public interface IGetEventDetailsRepository
{
    Task<Event?> GetEventDetailsAsync(Guid eventId, CancellationToken cancellationToken = default);
}

public class GetEventDetailsRepository : IGetEventDetailsRepository
{
    private readonly ApplanderDbContext _dbContext;

    public GetEventDetailsRepository(ApplanderDbContext dbContext)
        => _dbContext = dbContext;

    public async Task<Event?> GetEventDetailsAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Events.AsNoTracking()
            .Include(x => x.Invitations)
            .ThenInclude(x => x.ApplendarUser)
            .FirstOrDefaultAsync(x => x.Id == eventId, cancellationToken);
    }
}
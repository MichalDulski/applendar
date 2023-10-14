using Applander.Domain.Common;
using Applander.Domain.Entities;
using Applander.Infrastructure;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Applendar.API.V1.Features.Events;

[ApiController]
[ApiVersion(1.0)]
[Route("api/events")]
public class GetEventDetailsController : ControllerBase
{
    private readonly ILogger<GetEventDetailsController> _logger;
    private readonly IGetEventDetailsRepository _getEventDetailsRepository;

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
        var eventDetails = await _getEventDetailsRepository.GetEventDetailsAsync(eventId);

        if (eventDetails is null)
        {
            return BadRequest("Not found");
        }
        
        return Ok(new GetEventDetailsResult(eventDetails.Id, eventDetails.Name, eventDetails.StartAtUtc,
            eventDetails.Location, eventDetails.EventType, eventDetails.OrganizerId,
            eventDetails.MaximumNumberOfParticipants, eventDetails.IsCompanionAllowed, eventDetails.IsPetAllowed,
            eventDetails.Image != null ? Convert.ToBase64String(eventDetails.Image) : null));
    }
}

public record GetEventDetailsResult(Guid Id,
    string Name,
    DateTime StartAtUtc,
    Location Location,
    EventType EventType,
    Guid OrganizerId,
    int? MaximumNumberOfParticipants = null,
    bool IsCompanionAllowed = false,
    bool IsPetAllowed = false,
    string? Base64Image = null);

public interface IGetEventDetailsRepository
{
    Task<Event?> GetEventDetailsAsync(Guid eventId, CancellationToken cancellationToken = default);
}

public class GetEventDetailsRepository : IGetEventDetailsRepository
{
    private readonly ApplanderDbContext _dbContext;

    public GetEventDetailsRepository(ApplanderDbContext dbContext) { _dbContext = dbContext; }

    public async Task<Event?> GetEventDetailsAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId, cancellationToken);
    }
}
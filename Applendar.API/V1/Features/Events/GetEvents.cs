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
public class GetEventsController : ControllerBase
{
    private readonly ILogger<GetEventsController> _logger;
    private readonly IGetEventsRepository _getEventsRepository;

    public GetEventsController(ILogger<GetEventsController> logger, IGetEventsRepository getEventsRepository)
    {
        _logger = logger;
        _getEventsRepository = getEventsRepository;
    }

    [HttpGet]
    public async Task<ActionResult<GetEventDto[]>> Get([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate,
        [FromQuery] bool withArchived = false)
    {
        _logger.LogInformation("Get events");
        var events = await _getEventsRepository.GetEventsInRangeAsync(fromDate, toDate, withArchived);
        
        var eventsDtos = events.Select(x =>
        {
            var image = x.Image != null ? Convert.ToBase64String(x.Image) : null;

            return new GetEventDto(x.Id, x.Name, x.StartAtUtc,
                x.Location, x.EventType, x.OrganizerId, x.MaximumNumberOfParticipants,
                x.IsCompanionAllowed, x.IsPetAllowed, image);
        }).ToList();
        return Ok(eventsDtos);
    }
}

public record GetEventDto(Guid Id, string Name,
    DateTime StartAtUtc,
    Location Location,
    EventType EventType,
    Guid OrganizerId,
    int? MaximumNumberOfParticipants = null,
    bool IsCompanionAllowed = false,
    bool IsPetAllowed = false,
    string? Base64Image = null);

public interface IGetEventsRepository
{
    Task<ICollection<Event>> GetEventsInRangeAsync(DateTime? fromDate, DateTime? toDate,
        bool withArchived = false, CancellationToken cancellationToken = default);
}

public class GetEventsRepository : IGetEventsRepository
{
    private readonly ApplanderDbContext _dbContext;

    public GetEventsRepository(ApplanderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ICollection<Event>> GetEventsInRangeAsync(DateTime? fromDate, DateTime? toDate,
        bool withArchived = false, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Events.AsQueryable();
        
        if (!withArchived)
        {
            query = query.Where(x => !x.ArchivedAtUtc.HasValue);
        }
        
        if (fromDate != null)
        {
            query = query.Where(x => x.StartAtUtc.Date >= fromDate.Value.Date);
        }

        if (toDate != null)
        {
            query = query.Where(x => x.StartAtUtc.Date <= toDate.Value.Date);
        }
        
        return await query.ToListAsync(cancellationToken);
    }
}
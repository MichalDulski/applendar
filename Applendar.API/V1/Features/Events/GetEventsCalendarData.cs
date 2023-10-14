using Applander.Domain.Common;
using Applander.Domain.Entities;
using Applander.Infrastructure;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Applendar.API.V1.Features.Events;

[ApiController]
[ApiVersion(1.0)]
[Route("api/calendar/events")]
public class GetEventsCalendarDataController : ControllerBase
{
    private readonly ILogger<GetEventsController> _logger;
    private readonly IGetEventsCalendarDataRepository _getEventsRepository;

    public GetEventsCalendarDataController(ILogger<GetEventsController> logger,
        IGetEventsCalendarDataRepository getEventsRepository)
    {
        _logger = logger;
        _getEventsRepository = getEventsRepository;
    }

    [HttpGet]
    public async Task<ActionResult<Dictionary<DateTime, List<GetEventCalendarDataDto>>>> Get([FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] bool withArchived = false)
    {
        _logger.LogInformation("Get events calendar data");
        var events = await _getEventsRepository.GetEventsInRangeAsync(fromDate, toDate, withArchived);

        var eventsDto = events.Select(x =>
            {
                return new GetEventCalendarDataDto(x.Id, x.Name, x.StartAtUtc,
                    x.Location, x.EventType, x.OrganizerId,
                    x.MaximumNumberOfParticipants, x.IsCompanionAllowed, x.IsPetAllowed);
            })
            .GroupBy(x => x.StartAtUtc)
            .OrderBy(x => x.Key)
            .ToDictionary(x => x.Key, x => x.ToList());

        return Ok(eventsDto);
    }
}

public record GetEventsCalendarDataResult(Dictionary<DateTime, List<GetEventCalendarDataDto>> Dates);

public record GetEventCalendarDataDto(Guid Id,
    string Name,
    DateTime StartAtUtc,
    Location Location,
    EventType EventType,
    Guid OrganizerId,
    int? MaximumNumberOfParticipants = null,
    bool IsCompanionAllowed = false,
    bool IsPetAllowed = false);

public interface IGetEventsCalendarDataRepository
{
    Task<ICollection<Event>> GetEventsInRangeAsync(DateTime? fromDate,
        DateTime? toDate,
        bool withArchived = false,
        CancellationToken cancellationToken = default);
}

public class GetEventsCalendarDataRepository : IGetEventsCalendarDataRepository
{
    private readonly ApplanderDbContext _dbContext;

    public GetEventsCalendarDataRepository(ApplanderDbContext dbContext) { _dbContext = dbContext; }

    public async Task<ICollection<Event>> GetEventsInRangeAsync(DateTime? fromDate,
        DateTime? toDate,
        bool withArchived = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Event.AsQueryable();

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

        return await query.ToListAsync();
    }
}
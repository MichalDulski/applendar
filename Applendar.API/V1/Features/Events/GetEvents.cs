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
    public async Task<ActionResult<string>> Get()
    {
        _logger.LogInformation("Get events");
        var events = await _getEventsRepository.GetEvents();
        
        var eventsDto = events.Select(x => new GetEventDto(x.Id, x.Name)).ToList();
        return Ok(new GetEventsResult(eventsDto));
    }
}

public record GetEventsResult(ICollection<GetEventDto> Events);

public record GetEventDto(Guid Id, string Name);

public interface IGetEventsRepository
{
    Task<ICollection<Event>> GetEvents();
}

public class GetEventsRepository : IGetEventsRepository
{
    private readonly ApplanderDbContext _dbContext;

    public GetEventsRepository(ApplanderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ICollection<Event>> GetEvents()
    {
        return await _dbContext.Event.ToListAsync();
    }
}
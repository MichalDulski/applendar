using Applander.Domain.Common;
using Applander.Domain.Entities;
using Applander.Infrastructure;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Applendar.API.V1.Features.Events;

[ApiController]
[ApiVersion(1.0)]
[Route("api/events")]
public class AddEventsController : ControllerBase
{
    private readonly IAddEventRepository _addEventRepository;
    private readonly ILogger<AddEventsController> _logger;

    public AddEventsController(ILogger<AddEventsController> logger, IAddEventRepository addEventRepository)
    {
        _logger = logger;
        _addEventRepository = addEventRepository;
    }

    [HttpPost]
    public async Task<ActionResult<AddEventResponse>> Post([FromBody] AddEventRequest request)
    {
        _logger.LogInformation("Adding event");

        byte[]? image = request.Base64Image != null ? Convert.FromBase64String(request.Base64Image) : null;

        var @event = Event.Create(request.Name, request.StartAtUtc, request.Location,
            request.EventType, request.MaximumNumberOfParticipants, request.IsCompanionAllowed,
            request.IsPetAllowed, image);

        _addEventRepository.AddEvent(@event);
        await _addEventRepository.SaveChangesAsync();

        var response = new AddEventResponse(@event.Id, @event.Name);

        return Ok(response);
    }
}

public record AddEventRequest(string Name,
    DateTime StartAtUtc,
    Location Location,
    EventType EventType,
    int? MaximumNumberOfParticipants = null,
    bool IsCompanionAllowed = false,
    bool IsPetAllowed = false,
    string? Base64Image = null);

public record AddEventResponse(Guid Id, string Name);

public interface IAddEventRepository
{
    void AddEvent(Event @event);

    Task SaveChangesAsync();
}

public class AddEventRepository : IAddEventRepository
{
    private readonly ApplanderDbContext _dbContext;

    public AddEventRepository(ApplanderDbContext dbContext)
        => _dbContext = dbContext;

    public void AddEvent(Event @event) { _dbContext.Event.Add(@event); }

    public async Task SaveChangesAsync() { await _dbContext.SaveChangesAsync(); }
}
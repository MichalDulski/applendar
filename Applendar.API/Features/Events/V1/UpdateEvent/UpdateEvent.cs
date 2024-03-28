using Applendar.Domain.Common;
using Applendar.Domain.Entities;
using Applendar.Infrastructure;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Applendar.API.Features.Events.V1.UpdateEvent;

[ApiController]
[ApiVersion(1.0)]
[Route("api/events")]
[Authorize]
public class UpdateEventController : ControllerBase
{
    private readonly ILogger<UpdateEventController> _logger;
    private readonly IUpdateEventRepository _updateEventRepository;

    public UpdateEventController(ILogger<UpdateEventController> logger, IUpdateEventRepository updateEventRepository)
    {
        _logger = logger;
        _updateEventRepository = updateEventRepository;
    }

    [HttpPut("{eventId}")]
    public async Task<ActionResult<UpdateEventResponse>> Put([FromRoute] Guid eventId,
        [FromBody] UpdateEventRequest request)
    {
        _logger.LogInformation("Update an event");

        Event? @event = await _updateEventRepository.GetEventAsync(eventId);

        if (@event is null)
            return BadRequest("Not found");

        byte[]? image = request.Base64Image != null ? Convert.FromBase64String(request.Base64Image) : null;

        @event.Update(request.Name, request.StartAtUtc, request.Location,
            request.EventType, request.MaximumNumberOfParticipants, request.IsCompanionAllowed,
            request.IsPetAllowed, image);

        await _updateEventRepository.SaveChangesAsync();

        var response = new UpdateEventResponse(@event.Id, @event.Name);

        return Ok(response);
    }
}

public record UpdateEventRequest(string Name,
    DateTime StartAtUtc,
    Location Location,
    EventType EventType,
    int? MaximumNumberOfParticipants = null,
    bool IsCompanionAllowed = false,
    bool IsPetAllowed = false,
    string? Base64Image = null);

public record UpdateEventResponse(Guid Id, string Name);

public interface IUpdateEventRepository
{
    Task<Event?> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default);

    Task<ApplendarUser?> GetEventOrganizer(Guid organizerId, CancellationToken cancellationToken = default);

    Task SaveChangesAsync();
}

internal class UpdateEventRepository : IUpdateEventRepository
{
    private readonly ApplendarDbContext _dbContext;

    public UpdateEventRepository(ApplendarDbContext dbContext)
        => _dbContext = dbContext;

    public async Task<Event?> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default)
        => await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId, cancellationToken);

    public async Task<ApplendarUser?> GetEventOrganizer(Guid organizerId, CancellationToken cancellationToken = default)
        => await _dbContext.ApplendarUsers.FirstOrDefaultAsync(x => x.Id == organizerId, cancellationToken);

    public async Task SaveChangesAsync() { await _dbContext.SaveChangesAsync(); }
}
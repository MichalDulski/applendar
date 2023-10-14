using Applander.Domain.Entities;
using Applander.Infrastructure;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Applendar.API.V1.Features.Events;

[ApiController]
[ApiVersion(1.0)]
[Route("api/events")]
public class DeleteEventController : ControllerBase
{
    private readonly IDeleteEventRepository _deleteEventRepository;
    private readonly ILogger<DeleteEventController> _logger;

    public DeleteEventController(ILogger<DeleteEventController> logger, IDeleteEventRepository deleteEventRepository)
    {
        _logger = logger;
        _deleteEventRepository = deleteEventRepository;
    }

    [HttpDelete("{eventId}")]
    public async Task<ActionResult<DeleteEventResponse>> Delete([FromRoute] Guid eventId, [FromQuery] Guid organizerId)
    {
        _logger.LogInformation("Deleting an event");

        Event? @event = await _deleteEventRepository.GetEventAsync(eventId);

        if (@event is null)
            return BadRequest("Not found");

        ApplendarUser? organizer = await _deleteEventRepository.GetEventOrganizer(organizerId);

        if (organizer is null)
            return BadRequest("User not found");

        if (organizer.Id != organizerId)
            return Forbid();

        @event.Archive();

        await _deleteEventRepository.SaveChangesAsync();

        var response = new DeleteEventResponse(@event.Id, @event.Name);

        return Ok(response);
    }
}

public record DeleteEventResponse(Guid Id, string Name);

public interface IDeleteEventRepository
{
    Task<Event?> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default);

    Task<ApplendarUser?> GetEventOrganizer(Guid organizerId, CancellationToken cancellationToken = default);

    Task SaveChangesAsync();
}

public class DeleteEventRepository : IDeleteEventRepository
{
    private readonly ApplanderDbContext _dbContext;

    public DeleteEventRepository(ApplanderDbContext dbContext)
        => _dbContext = dbContext;

    public async Task<Event?> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default)
        => await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId, cancellationToken);

    public async Task<ApplendarUser?> GetEventOrganizer(Guid organizerId, CancellationToken cancellationToken = default)
        => await _dbContext.ApplendarUsers.FirstOrDefaultAsync(x => x.Id == organizerId, cancellationToken);

    public async Task SaveChangesAsync() { await _dbContext.SaveChangesAsync(); }
}

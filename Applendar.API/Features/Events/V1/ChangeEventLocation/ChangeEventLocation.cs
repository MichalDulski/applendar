using Applendar.Domain.Entities;
using Applendar.Infrastructure;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Applendar.API.Features.Events.V1.ChangeEventLocation;

[ApiController]
[ApiVersion(1.0)]
[Route("api/events")]
[Authorize]
public class ChangeEventLocationController : ControllerBase
{
    private readonly ILogger<ChangeEventLocationController> _logger;
    private readonly IChangeEventLocationRepository _updateEventRepository;

    public ChangeEventLocationController(ILogger<ChangeEventLocationController> logger, IChangeEventLocationRepository updateEventRepository)
    {
        _logger = logger;
        _updateEventRepository = updateEventRepository;
    }

    [HttpPut("{eventId}/location")]
    public async Task<ActionResult<ChangeEventLocationResponse>> Put([FromRoute] Guid eventId,
        [FromBody] ChangeEventLocationRequest request)
    {
        _logger.LogInformation("Update an event");

        Location location = request.Location;
        
        Event? @event = await _updateEventRepository.GetEventAsync(eventId);

        if (@event is null)
            return BadRequest("Not found");

        @event.ChangeLocation(location);

        await _updateEventRepository.SaveChangesAsync();

        var response = new ChangeEventLocationResponse(@event.Id, @event.Name);

        return Ok(response);
    }
}

public record ChangeEventLocationRequest(Location Location);

public record ChangeEventLocationResponse(Guid Id, string Name);

public interface IChangeEventLocationRepository
{
    Task<Event?> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default);

    Task<ApplendarUser?> GetEventOrganizer(Guid organizerId, CancellationToken cancellationToken = default);

    Task SaveChangesAsync();
}

internal class ChangeEventLocationRepository : IChangeEventLocationRepository
{
    private readonly ApplendarDbContext _dbContext;

    public ChangeEventLocationRepository(ApplendarDbContext dbContext)
        => _dbContext = dbContext;

    public async Task<Event?> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default)
        => await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId, cancellationToken);

    public async Task<ApplendarUser?> GetEventOrganizer(Guid organizerId, CancellationToken cancellationToken = default)
        => await _dbContext.ApplendarUsers.FirstOrDefaultAsync(x => x.Id == organizerId, cancellationToken);

    public async Task SaveChangesAsync() { await _dbContext.SaveChangesAsync(); }
}
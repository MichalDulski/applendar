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
public class AddEventController : ControllerBase
{
    private readonly IAddEventRepository _addEventRepository;
    private readonly ILogger<AddEventController> _logger;

    public AddEventController(ILogger<AddEventController> logger, IAddEventRepository addEventRepository)
    {
        _logger = logger;
        _addEventRepository = addEventRepository;
    }

    [HttpPost]
    public async Task<ActionResult<AddEventResponse>> Post([FromBody] AddEventRequest request)
    {
        _logger.LogInformation("Adding event");

        var eventOrganizer = await _addEventRepository.GetEventOrganizer(request.OrganizerId);

        if (eventOrganizer is null)
        {
            return BadRequest("User not found");
        }
        
        byte[]? image = request.Base64Image != null ? Convert.FromBase64String(request.Base64Image) : null;

        var @event = Event.Create(request.Name, request.StartAtUtc, request.Location,
            request.EventType, eventOrganizer, request.MaximumNumberOfParticipants, request.IsCompanionAllowed,
            request.IsPetAllowed, image);

        _addEventRepository.AddEvent(@event);

        var appUsers = await _addEventRepository.GetAllUsersAsync();

        foreach (var appUser in appUsers)
        {
            @event.InviteUser(appUser);
        }
        
        await _addEventRepository.SaveChangesAsync();

        var response = new AddEventResponse(@event.Id, @event.Name);

        return Ok(response);
    }
}

public record AddEventRequest(string Name,
    DateTime StartAtUtc,
    Location Location,
    EventType EventType,
    Guid OrganizerId,
    int? MaximumNumberOfParticipants = null,
    bool IsCompanionAllowed = false,
    bool IsPetAllowed = false,
    string? Base64Image = null);

public record AddEventResponse(Guid Id, string Name);

public interface IAddEventRepository
{
    void AddEvent(Event @event);

    Task<ApplendarUser?> GetEventOrganizer(Guid organizerId, CancellationToken cancellationToken = default);
    Task<ICollection<ApplendarUser>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync();
}

public class AddEventRepository : IAddEventRepository
{
    private readonly ApplanderDbContext _dbContext;

    public AddEventRepository(ApplanderDbContext dbContext)
        => _dbContext = dbContext;

    public void AddEvent(Event @event) { _dbContext.Events.Add(@event); }

    public async Task<ApplendarUser?> GetEventOrganizer(Guid organizerId, CancellationToken cancellationToken = default)
        => await _dbContext.ApplendarUsers.FirstOrDefaultAsync(x => x.Id == organizerId, cancellationToken);

    public async Task<ICollection<ApplendarUser>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        => await _dbContext.ApplendarUsers.ToListAsync(cancellationToken);

    public async Task SaveChangesAsync() { await _dbContext.SaveChangesAsync(); }
}
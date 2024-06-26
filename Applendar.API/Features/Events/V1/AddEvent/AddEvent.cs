using Applendar.Domain.Common;
using Applendar.Domain.Entities;
using Applendar.Infrastructure;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Applendar.API.Features.Events.V1.AddEvent;

[ApiController]
[ApiVersion(1.0)]
[Route("api/events")]
[Authorize]
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

        ApplendarUser? eventOrganizer = await _addEventRepository.GetEventOrganizer(request.OrganizerId);

        if (eventOrganizer is null)
            return BadRequest("User not found");

        byte[]? image = request.Base64Image != null ? Convert.FromBase64String(request.Base64Image) : null;

        var @event = Event.Create(request.Name, request.StartAtUtc, request.Location,
            request.EventType, eventOrganizer, request.MaximumNumberOfParticipants,
            request.IsCompanionAllowed, request.IsPetAllowed, image);

        _addEventRepository.AddEvent(@event);

        ICollection<ApplendarUser> appUsers = await _addEventRepository.GetAllUsersAsync();

        foreach (ApplendarUser appUser in appUsers)
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

    Task<ICollection<ApplendarUser>> GetAllUsersAsync(CancellationToken cancellationToken = default);

    Task<ApplendarUser?> GetEventOrganizer(Guid organizerId, CancellationToken cancellationToken = default);

    Task SaveChangesAsync();
}

internal class AddEventRepository : IAddEventRepository
{
    private readonly ApplendarDbContext _dbContext;

    public AddEventRepository(ApplendarDbContext dbContext)
        => _dbContext = dbContext;

    public void AddEvent(Event @event) { _dbContext.Events.Add(@event); }

    public async Task<ICollection<ApplendarUser>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        => await _dbContext.ApplendarUsers.ToListAsync(cancellationToken);

    public async Task<ApplendarUser?> GetEventOrganizer(Guid organizerId, CancellationToken cancellationToken = default)
        => await _dbContext.ApplendarUsers.FirstOrDefaultAsync(x => x.Id == organizerId, cancellationToken);

    public async Task SaveChangesAsync() { await _dbContext.SaveChangesAsync(); }
}
using Applander.Domain.Common;

namespace Applander.Domain.Entities;

public class Event : BaseEntity
{
    public Event() { }
    
    public string Name { get; set; }
    public DateTime StartAtUtc { get; set; }
    public Location Location { get; set; }
    public int? MaximumNumberOfParticipants { get; set; }
    public bool IsNumberOfParticipantsLimited => MaximumNumberOfParticipants.HasValue;
    public bool IsCompanionAllowed { get; set; }
    public bool IsPetAllowed { get; set; }
    public byte[]? Image { get; set; }
    public EventType EventType { get; set; }
    
    public bool IsStarted => StartAtUtc <= DateTime.UtcNow;
    public bool IsArchived => ArchivedAtUtc.HasValue;
    
    public static Event Create(string name, DateTime startAtUtc, Location location,
        EventType eventType, int? MaximumNumberOfParticipants = null, bool isCompanionAllowed = false,
        bool isPetAllowed = false, byte[]? image = null)
        => new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            StartAtUtc = startAtUtc,
            Location = location,
            EventType = eventType,
            MaximumNumberOfParticipants = MaximumNumberOfParticipants,
            IsCompanionAllowed = isCompanionAllowed,
            IsPetAllowed = isPetAllowed,
            Image = image
            
        };
}

public record Location(string Name, string City, string ZipCode, string Address, string Country);
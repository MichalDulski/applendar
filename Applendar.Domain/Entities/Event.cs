using Applander.Domain.Common;

namespace Applander.Domain.Entities;

public sealed class Event : BaseEntity
{
    private Event() { }
    
    private Event(Guid id,
        string name,
        DateTime startAtUtc,
        Location location,
        EventType eventType,
        Guid organizerId,
        int? maximumNumberOfParticipants = null,
        bool isCompanionAllowed = false,
        bool isPetAllowed = false,
        byte[]? image = null)
    {
        Id = id;
        Name = name;
        StartAtUtc = startAtUtc;
        Location = location;
        EventType = eventType;
        OrganizerId = organizerId;
        MaximumNumberOfParticipants = maximumNumberOfParticipants;
        IsCompanionAllowed = isCompanionAllowed;
        IsPetAllowed = isPetAllowed;
        Image = image;
    }

    public bool IsArchived => ArchivedAtUtc.HasValue;
    public bool IsNumberOfParticipantsLimited => MaximumNumberOfParticipants.HasValue;

    public bool IsStarted => StartAtUtc <= DateTime.UtcNow;
    public EventType EventType { get; set; }
    public byte[]? Image { get; set; }
    public ICollection<EventInvitation> Invitations { get; set; } = new List<EventInvitation>();
    public bool IsCompanionAllowed { get; set; }
    public bool IsPetAllowed { get; set; }
    public Location Location { get; set; }
    public int? MaximumNumberOfParticipants { get; set; }
    public string Name { get; set; }
    public ApplendarUser Organizer { get; set; }
    public Guid OrganizerId { get; set; }
    public DateTime StartAtUtc { get; set; }

    public static Event Create(string name,
        DateTime startAtUtc,
        Location location,
        EventType eventType,
        ApplendarUser organizer,
        int? maximumNumberOfParticipants = null,
        bool isCompanionAllowed = false,
        bool isPetAllowed = false,
        byte[]? image = null)
        => new(Guid.Empty, name, startAtUtc,
            location, eventType, organizer.Id,
            maximumNumberOfParticipants, isCompanionAllowed, isPetAllowed,
            image);

    public void Archive()
        => ArchivedAtUtc = DateTime.UtcNow;

    public void InviteUser(ApplendarUser user)
    {
        var invitation = EventInvitation.Create(user, this);
        Invitations.Add(invitation);
    }

    public void Update(string name,
        DateTime startAtUtc,
        Location location,
        EventType eventType,
        int? maximumNumberOfParticipants = null,
        bool isCompanionAllowed = false,
        bool isPetAllowed = false,
        byte[]? image = null)
    {
        Name = name;
        StartAtUtc = startAtUtc;
        Location = location;
        EventType = eventType;
        MaximumNumberOfParticipants = maximumNumberOfParticipants;
        IsCompanionAllowed = isCompanionAllowed;
        IsPetAllowed = isPetAllowed;
        Image = image;
    }

    public void ChangeLocation(Location location)
        => Location = location;
}

public record Location(bool IsOnline,
    string? Url = null,
    string? Name = null,
    string? City = null,
    string? ZipCode = null,
    string? Address = null,
    string? Country = null);
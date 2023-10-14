using Applander.Domain.Common;

namespace Applander.Domain.Entities;

public class Event : BaseEntity
{
    public Event() { }

    public Guid OrganizerId { get; set; }
    public virtual ApplendarUser Organizer { get; set; }
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

    public static Event Create(string name,
        DateTime startAtUtc,
        Location location,
        EventType eventType,
        ApplendarUser organizer,
        int? maximumNumberOfParticipants = null,
        bool isCompanionAllowed = false,
        bool isPetAllowed = false,
        byte[]? image = null)
        => new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            StartAtUtc = startAtUtc,
            Location = location,
            EventType = eventType,
            MaximumNumberOfParticipants = maximumNumberOfParticipants,
            IsCompanionAllowed = isCompanionAllowed,
            IsPetAllowed = isPetAllowed,
            Image = image,
            Organizer = organizer
        };
}

public record Location
{
    public Location(bool isOnline,
        string? url = null,
        string? name = null,
        string? city = null,
        string? zipCode = null,
        string? address = null,
        string? country = null)
    {
        Name = name;
        IsOnline = isOnline;
        Url = url;
        City = city;
        ZipCode = zipCode;
        Address = address;
        Country = country;
    }

    public string? Name { get; init; }
    public string? City { get; init; }
    public string? ZipCode { get; init; }
    public string? Address { get; init; }
    public string? Country { get; init; }
    public bool IsOnline { get; set; }
    public string? Url { get; set; } = string.Empty;

    public void Deconstruct(out bool isOnline,
        out string? url,
        out string? name,
        out string? city,
        out string? zipCode,
        out string? address,
        out string? country)
    {
        isOnline = IsOnline;
        url = Url;
        name = Name;
        city = City;
        zipCode = ZipCode;
        address = Address;
        country = Country;
    }
}
using Applendar.Domain.Common;

namespace Applendar.Domain.Entities;

public sealed class ApplendarUser : BaseEntity
{
    private ApplendarUser() { }

    private ApplendarUser(Guid id,
        string externalId,
        string firstName,
        string lastName,
        Preferences preferences)
    {
        Id = id;
        ExternalId = externalId;
        FirstName = firstName;
        LastName = lastName;
        Preferences = preferences;
    }

    public ICollection<EventInvitation> EventInvitations { get; set; } = new List<EventInvitation>();

    // Auth0 user id/subject
    public string ExternalId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ICollection<Event> OrganizedEvents { get; set; } = new List<Event>();

    public Preferences Preferences { get; set; }
    
    public DateTime LastActivityDateUtc { get; set; }

    public static ApplendarUser Create(string firstName,
        string lastName,
        string externalId)
        => new(Guid.Empty, firstName, lastName,
            externalId, new Preferences(true, true, true,
                true));

    public void UpdateUserPreferences(Preferences preferences) { Preferences = preferences; }

    public void LogActivity()
    {
        LastActivityDateUtc = DateTime.UtcNow;
    }

}

public record Preferences(bool NotifyAboutOnlineEvents,
    bool NotifyAboutOfflineEvents,
    bool NotifyAboutEventsWithPets,
    bool NotifyAboutEventsWithCompanions);
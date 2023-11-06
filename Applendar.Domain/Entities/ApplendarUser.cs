using Applander.Domain.Common;

namespace Applander.Domain.Entities;

public class ApplendarUser : BaseEntity
{
    public virtual ICollection<EventInvitation> EventInvitations { get; set; } = new List<EventInvitation>();

    // Auth0 user id/subject
    public string ExternalId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public virtual ICollection<Event> OrganizedEvents { get; set; } = new List<Event>();

    public Preferences Preferences { get; set; }
    
    public DateTime LastActivityDateUtc { get; set; }

    public static ApplendarUser Create(string firstName,
        string lastName,
        string externalId)
        => new()
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            ExternalId = externalId,
            Preferences = new Preferences(true, true, true,
                true)
        };

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
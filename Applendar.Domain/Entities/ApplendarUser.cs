using Applander.Domain.Common;

namespace Applander.Domain.Entities;

public class ApplendarUser : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    // Auth0 user id/subject
    public string ExternalId { get; set; }
    
    public virtual ICollection<Event> OrganizedEvents { get; set; } = new List<Event>();
    
    public static ApplendarUser Create(string firstName, string lastName, string externalId)
        => new()
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            ExternalId = externalId
        };
}
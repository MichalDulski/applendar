using Applander.Domain.Common;

namespace Applander.Domain.Entities;

public class EventInvitation : Updateable
{
    public Guid ApplendarUserId { get; set; }
    public Guid EventId { get; set; }
    public ApplendarUser ApplendarUser { get; set; }
    public Event Event { get; set; }
    public InvitationStatus Status { get; set; }
}
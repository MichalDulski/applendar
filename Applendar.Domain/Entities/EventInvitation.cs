using Applendar.Domain.Common;

namespace Applendar.Domain.Entities;

public class EventInvitation : Updateable
{
    public ApplendarUser ApplendarUser { get; set; }
    public Guid ApplendarUserId { get; set; }
    public Event Event { get; set; }
    public Guid EventId { get; set; }
    public InvitationStatus Status { get; set; }

    public static EventInvitation Create(ApplendarUser applendarUser, Event @event)
        => new() { ApplendarUserId = applendarUser.Id, EventId = @event.Id, Status = InvitationStatus.Pending };

    public void ChangeInvitationStatus(InvitationStatus status) { Status = status; }
}
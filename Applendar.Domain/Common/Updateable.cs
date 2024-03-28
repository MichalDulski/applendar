namespace Applendar.Domain.Common;

public abstract class Updateable
{
    public DateTime? ArchivedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}

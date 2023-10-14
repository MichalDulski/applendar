namespace Applander.Domain.Common;

public abstract class BaseEntity : Updateable
{
    public Guid Id { get; set; }
}

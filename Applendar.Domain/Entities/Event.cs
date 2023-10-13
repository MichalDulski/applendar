namespace Applander.Domain.Entities;

public class Event
{
    public Event() { }
    
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    public static Event Create(string name)
        => new()
        {
            Id = Guid.NewGuid(),
            Name = name
        };
}
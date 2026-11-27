using System;

namespace Hotel_app.Classes;

public class Entity
{
    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; private set; }

    public Entity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }

    protected void UpdateTimestamp()
    {
        UpdatedAt = DateTime.Now;
    }

    public virtual string Describe()
    {
        return $"Entity ID: {Id}";
    }

    public override string ToString() => Describe();
}
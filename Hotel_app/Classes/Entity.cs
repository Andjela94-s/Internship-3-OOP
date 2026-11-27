namespace Hotel_app.Classes;

public class Entity
{
    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public DateTime UpdatedAt { get; init; }

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
}
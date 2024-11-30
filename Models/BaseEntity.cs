namespace MessengerAPI.Models;

// Base Entity 
// - Provides common properties for all domain entities
// - Provides generic type for the ID limited to value types (int, long, etc.)
public abstract class BaseEntity<T> where T : struct
{
    // Internal ID for database efficiency
    public T Id { get; set; }
    
    // External ID (GUID) for API exposure
    public Guid Guid { get; set; } = Guid.NewGuid();
    
    // Audit field for tracking entity creation
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
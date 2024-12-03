namespace MessengerAPI.Models;

// Base Entity 
// - Provides common properties for all domain entities
// - Provides generic type for the ID limited to value types (int, long, etc.)
public abstract class BaseEntity<T> where T : struct
{
    // Shared Properties ------------------------------------------
    
    // Database Identity (Primary Key)
    // Set by EF Core when the object is persisted to the database
    public T Id { get; private set; }
    
    // Domain Identity (Global Unique Identifier)
    // Can only be set during object initialization
    public Guid Guid { get; init; } = Guid.NewGuid();
    
    // CreatedAt (Audit Field)
    // Can only be set during object initialization
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    
    // Updated At (Audit Field)
    // Set by EF Core when the object is updated in the database
    public DateTime? UpdatedAt { get; set; }
    
    
    // Domain Identity (Guid) ------------------------------------------
    
    public override bool Equals(object? obj)
    {
        if (obj is not BaseEntity<T> other)
            return false;

        return Guid == other.Guid;
    }

    public override int GetHashCode()
    {
        return Guid.GetHashCode();
    }

    public static bool operator ==(BaseEntity<T>? left, BaseEntity<T>? right)
    {
        if (ReferenceEquals(left, right))
            return true;
        if (left is null || right is null)
            return false;
        return left.Equals(right);
    }

    public static bool operator !=(BaseEntity<T>? left, BaseEntity<T>? right)
    {
        return !(left == right);
    }
    
}
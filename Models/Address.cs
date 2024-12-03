namespace MessengerAPI.Models;

// - Value Object
// EF Core: Owned Entity Type (see MessengerContext.cs)
public record Address(string Street, string City, string Country);
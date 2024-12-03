namespace MessengerAPI.Models;

// User Entity
// - Represents a user in the system
public class User : BaseEntity<int>
{
    // Properties ------------------------------------------
    
    // Email should be unique
    public string Email { get; set; }
    
    // Password should be hashed and salted
    // TODO: https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html
    public string Password { get; set; }
    
    // Address of the user
    public Address Address { get; set; }
    
    
    // Constructors ------------------------------------------
    public User(string email, string password, Address address)
    {
        Email = email;
        Password = password;
        Address = address;
    }
    
    // Constructor for EF Core
    protected User() { }
}
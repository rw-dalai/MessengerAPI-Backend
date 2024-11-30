namespace MessengerAPI.Models;

// User Entity
// - Represents a user in the system
// - Inherits from BaseEntity with an integer ID
public class User : BaseEntity<int>
{
    // Email should be unique
    public string Email { get; set; }
    
    // Password should be hashed and salted
    // TODO: https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html
    public string Password { get; set; }
    

    // Constructor for use Developers
    //   when creating a new `User` e.g. in a registration process
    public User(string email, string password)
    {
        Email = email;
        Password = password;
    }
    
    // Constructor for Entity Framework .NET Core
    //   when the `User` is materialized from the database into memory
    protected User() { }
}
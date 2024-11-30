namespace MessengerAPI.Models;

/*
- Navigation properties
  (e.g., Sender) simplify accessing related data in EF Core by allowing direct object references.

- Foreign key properties
   (e.g., SenderId) are used to establish relationships between entities in the database.

- Why include both?
    // Create a new message without loading the User object
   var message = new Message("Hello!", senderId: 123); 
   
   // Later, if we need the Sender, EF Core will automatically load it
   var sender = message.Sender;
   
- Virtual keyword?
    Allows EF Core to override the property to lazy-load the related entity when accessed.
   
   https://www.google.com/search?client=firefox-b-d&q=navigation+properties+EF+Core#fpstate=ive&vld=cid:f417b895,vid:1nsw7dlrbLI,st:0
*/


// Message Entity
// - Represents a message in a `MessengerEntry`
// - Inherits from `BaseEntity` with an integer ID
public class Message : BaseEntity<int>
{
    // Content of the message
    public string Content { get; set; }
    
    // Sender of the message
    private int SenderId { get; set; }
    public virtual User Sender { get; set; } = default!;
    
    // MessengerEntry that the message belongs to
    public int MessengerEntryId { get; set; }
    public virtual MessengerEntry MessengerEntry { get; set; } = default!;
    
    // Constructor for use Developers
    //   when creating a new `Message` e.g. sending a message to a MessengerEntry
    public Message(string content, int senderId, int messengerEntryId)
    {
        Content = content;
        SenderId = senderId;
        MessengerEntryId = messengerEntryId;
    }
    
    // Constructor for Entity Framework .NET Core
    //   when the `Message` is materialized from the database into memory
    protected Message() { }
}
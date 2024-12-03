namespace MessengerAPI.Models;

// Message Entity
// - Represents a message in a chat
public class Message : BaseEntity<int>
{
    // Properties ------------------------------------------
    
    // EF Core: Navigation property
    public virtual MessengerEntry MessengerEntry { get; private set; } = default!;
    
    // EF Core: Navigation property
    public virtual User Sender { get; private set; } = default!;
    
    // EF Core: Creates Shadow Property "SenderId"/"MessengerEntryId" behind the scenes
    // public int SenderId/MessengerEntryId { get; private init; }
    
    // EF Core automatically:
    // 1. Creates "SenderId"/"MessengerEntryId" foreign key in database
    // 2. Makes it required (NOT NULL) because property is non-nullable
    // 3. Creates index on the foreign key
    // 4. Sets up cascading delete by default
    
    // Question: What happens if we remove the MessengerEntry navigation property?
    // "MessengerEntryId" INTEGER NULL,
    //  CONSTRAINT "FK_Messages_MessengerEntries_MessengerEntryId" FOREIGN KEY ("MessengerEntryId") REFERENCES "MessengerEntries" ("Id")
    // There is not CASCADE DELETE because the relationship is optional.

    
    public string Content { get; private init; }
    
    
    // Constructors ------------------------------------------
    public Message(string content, User sender)
    {
        Content = content;
        Sender = sender;
    }
    
    // Constructor for Entity Framework
    protected Message() { }
}
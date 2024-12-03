namespace MessengerAPI.Models;

// MessengerEntry Entity
// - Represents a chat between users
public class MessengerEntry : BaseEntity<int>
{
    // Properties ------------------------------------------
    
    // EF Core: Navigation property
    public virtual User Owner { get; private set; } = default!;
    
    // EF Core: Creates Shadow Property "OwnerId" behind the scenes
    // public int OwnerId { get; private init; }
    
    // EF Core automatically:
    // 1. Creates "OwnerId" foreign key in database
    // 2. Makes it required (NOT NULL) because property is non-nullable
    // 3. Creates index on the foreign key
    // 4. Sets up cascading delete by default
    
    // Participants in the chat
    private readonly List<User> _participants = []; 
    public IReadOnlyList<User> Participants => _participants.AsReadOnly();
    
    // Messages in the chat
    private readonly List<Message> _messages = []; 
    public IReadOnlyList<Message> Messages => _messages.AsReadOnly();
    
    
    // Constructors ------------------------------------------
    public MessengerEntry(User owner, List<User> initialParticipants)
    {
        // Owner must not be in the participants list
        if (initialParticipants.Any(p => p.Guid == owner.Guid))
            throw new MessengerException("Owner must not be provided in the participants list");
            
        Owner = owner;
        _participants = initialParticipants;
    }
    
    // Constructor for EF Core
    protected MessengerEntry() { }
    
    
    // Domain Logic ------------------------------------------
    public void SendMessage(string content, User sender)
    {
        if (!IsParticipant(sender.Guid) && !IsOwner(sender.Guid))
            throw new MessengerException("User is not a member in this messenger");

        var message = new Message(content, sender);
        _messages.Add(message);
    }

    public void AddParticipant(User owner, User participantToAdd)
    {
        if (!IsOwner(owner.Guid))
            throw new MessengerException("Only the owner can perform this action");
        
        if (IsParticipant(participantToAdd.Guid))
            throw new MessengerException("User is already a participant");
        
        _participants.Add(participantToAdd);
    }
    
    public void RemoveParticipant(User owner, User participantToRemove)
    {
        if (!IsOwner(owner.Guid))
            throw new MessengerException("Only the owner can perform this action");
        
        if (!IsParticipant(participantToRemove.Guid))
            throw new MessengerException("User is not a participant in this messenger");
        
        _participants.Remove(participantToRemove);
    }
    
    
    // Private Helper Methods ------------------------------------------
    private bool IsParticipant(Guid userGuid) => 
        Participants.Any(p => p.Guid == userGuid);
        
    private bool IsOwner(Guid userGuid) => 
        Owner.Guid == userGuid;
}
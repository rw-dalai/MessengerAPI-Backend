namespace MessengerAPI.Models;

// MessengerEntry Entity
// - Core entity in our domain model
// - Represents a chat between `User`s
// - Inherits from `BaseEntity` with an integer ID
public class MessengerEntry : BaseEntity<int>
{
    // Properties ------------------------------------------
    
    // Owner is the creator of a `MessengeEntry`, only they can add/remove participants
    public int OwnerId { get; set; }
    public virtual User Owner { get; set; } = default!;
    
    // Who is in the chat (n:n relationship)
    private readonly List<User> _participants = []; // private readonly backing field
    public IReadOnlyList<User> Participants => _participants.AsReadOnly(); // public getter
    
    // Messages in the chat (1:n relationship)
    private readonly List<Message> _messages = []; // private readonly backing field
    public IReadOnlyList<Message> Messages => _messages.AsReadOnly(); // public getter
    
    
    // Constructors ------------------------------------------
    
    // Constructor for use Developers
    //   when creating a new `MessengerEntry` e.g. starting a new chat
    public MessengerEntry(User owner, List<User> initialParticipants)
    {
        OwnerId = owner.Id;
        _participants = initialParticipants;
        
        if (IsParticipant(owner.Id))
            throw new MessengerException("Owner must not be provided in the participants list");
    }
    
    // Constructor for Entity Framework .NET Core
    //   when the `MessengerEntry` is materialized from the database into memory
    protected MessengerEntry() { }
    
    
    // Domain Logic ------------------------------------------
    
    // Add Message to the chat
    public void SendMessage(string content, int senderId)
    {
        if (!IsParticipant(senderId) && !IsOwner(senderId))
            throw new MessengerException("User is not a member in this messenger");

        var message = new Message(content, senderId, Id);
        _messages.Add(message);
    }

    // Add one participant to the chat
    public void AddParticipant(int ownerId, User participantToAdd)
    {
        if (!IsOwner(ownerId))
            throw new MessengerException("Only the owner can perform this action");
        
        if (IsParticipant(participantToAdd.Id))
            throw new MessengerException("User is already a participant");
        
        _participants.Add(participantToAdd);
    }
    
    // Remove a participant from the chat
    public void RemoveParticipant(int ownerId, User participantToRemove)
    {
        if (!IsOwner(ownerId))
            throw new MessengerException("Only the owner can perform this action");
        
        if (!IsParticipant(participantToRemove.Id))
            throw new MessengerException("User is not a participant in this messenger");
        
        _participants.Remove(participantToRemove);
    }
    
    
    // Private Helper Methods ------------------------------------------

    private bool IsParticipant(int userId) => 
        Participants.Any(p => p.Id == userId);
        
    private bool IsOwner(int userId) => 
        OwnerId == userId;
}
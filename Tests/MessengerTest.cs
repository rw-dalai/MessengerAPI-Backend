using MessengerAPI.Infrastructure;
using MessengerAPI.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MessengerAPI.Tests;

// NugGet Packages
//   - Microsoft.Net.Test.Sdk
//   - Xunit

// Test Naming Convention
//  Should_[ExpectedBehavior]_When_[TestCondition]

public class MessengerTest
{
    // Test Fixtures ----------------------------------------------------------
    private readonly User _owner;
    private readonly User _participant1;
    private readonly User _participant2;
    private readonly User _nonParticipant;
    private readonly MessengerEntry _messenger;
    
    public MessengerTest()
    {
        _owner = new User("owner@test.com", "password", new Address("123 Street", "City", "12345"));
        _participant1 = new User("participant1@test.com", "password", new Address("123 Street", "City", "12345"));
        _participant2 = new User("participant2@test.com", "password", new Address("123 Street", "City", "12345"));
        _nonParticipant = new User("non@test.com", "password", new Address("123 Street", "City", "12345"));
        _messenger = new MessengerEntry(_owner, [_participant1]);
    }

    
    // Setup Test Database ----------------------------------------------------------
    private MessengerContext CreateContext()
    {
        // Setup SQLite in-memory database
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        // Setup Database Options like Logging for easier debugging
        var options = new DbContextOptionsBuilder<MessengerContext>()
            .UseSqlite(connection)
            .LogTo(Console.WriteLine)
            .Options;

        // Create the DbContext
        var context = new MessengerContext(options);

        // Start with empty database
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Seed Test Fixtures
        Seed(context);
        
        return context;
    }
    
    private void Seed(MessengerContext db)
    {
        db.Users.AddRange(_owner, _participant1, _participant2, _nonParticipant);
        db.MessengerEntries.Add(_messenger);
        // db.SaveChanges();
    }
    
    
    // Database Schema Tests ----------------------------------------------------------
    [Fact]
    public void Should_CreateDatabase()
    {
        using var db = CreateContext();
    }
    
    
    // Constructor Tests ----------------------------------------------------------
    [Fact]
    public void Should_ThrowException_When_OwnerInParticipants()
    {
        Assert.Throws<MessengerException>(() => 
            new MessengerEntry(_owner, [_owner, _participant1]));
    }
    
    
    [Fact]
    public void Should_CreateMessenger()
    {
        using var db = CreateContext();

        // Given: A new Messenger
        var messenger = new MessengerEntry(_owner, []);

        // When: Saving the Messenger to the Database
        db.MessengerEntries.Add(messenger);
        db.SaveChanges();
        
        // Then: The Messenger should be retrieved from the Database
        var retrievedMessenger = db.MessengerEntries.First(m => m.Id == messenger.Id);
        Assert.NotNull(retrievedMessenger);
    }

    
    // AddParticipant Tests ----------------------------------------------------------
    [Fact]
    public void Should_ThrowException_When_NonOwnerAddsParticipant()
    {
        Assert.Throws<MessengerException>(() => 
            _messenger.AddParticipant(_participant1, _participant2));
    }

    
    [Fact]
    public void Should_ThrowException_When_AddingExistingParticipant()
    {
        Assert.Throws<MessengerException>(() => 
            _messenger.AddParticipant(_owner, _participant1));
    }

    
    [Fact]
    public void Should_AddParticipantToMessenger()
    {
        using var db = CreateContext();
        
        // When: Adding a new Participant to the Messenger
        _messenger.AddParticipant(_owner, _participant2);
        db.SaveChanges();
        
        // Then: The Participant should be added the Database
        var retrievedMessenger = db.MessengerEntries.First(m => m.Id == _messenger.Id);
        Assert.Contains(retrievedMessenger.Participants, p => p.Id == _participant2.Id);
    }
    

    // RemoveParticipant Tests ----------------------------------------------------------
    [Fact]
    public void Should_ThrowException_When_NonOwnerRemovesParticipant()
    {
        Assert.Throws<MessengerException>(() => 
            _messenger.RemoveParticipant(_participant1, _participant1));
    }

    
    [Fact]
    public void Should_ThrowException_When_RemovingOwner()
    {
        Assert.Throws<MessengerException>(() => 
            _messenger.RemoveParticipant(_owner, _owner));
    }

    
    [Fact]
    public void Should_ThrowException_When_RemovingNonParticipant()
    {
        Assert.Throws<MessengerException>(() => 
            _messenger.RemoveParticipant(_owner, _nonParticipant));
    }

    
    [Fact]
    public void Should_RemoveExistingParticipant()
    {
        using var db = CreateContext();
        
        // When: Removing an existing Participant from the Messenger
        _messenger.RemoveParticipant(_owner, _participant1);
        db.SaveChanges();

        // Then: The Participant should be removed from Database
        var retrievedMessenger = db.MessengerEntries.First(m => m.Id == _messenger.Id);
        Assert.Empty(retrievedMessenger.Participants);
    }

    
    // SendMessage Tests ----------------------------------------------------------
    [Fact]
    public void Should_ThrowException_When_NotOwnerOrParticipantSendMessage()
    {
        Assert.Throws<MessengerException>(() =>
            _messenger.SendMessage("Test message", _nonParticipant));
    }

    
    [Fact]
    public void Should_SendMessage_When_OwnerSendsMessage()
    {
        using var db = CreateContext();
        
        // When: Sending a Message as the Owner
        _messenger.SendMessage("Test message", _owner);
        db.SaveChanges();
        
        // Then: Message should be added to the Database
        var retrievedMessenger = db.MessengerEntries.First(m => m.Id == _messenger.Id);
        Assert.Single(retrievedMessenger.Messages);
    }
    
    
    [Fact]
    public void Should_SendMessageAsMember()
    {
        using var db = CreateContext();
        
        // When: Sending a Message as a Participant
        _messenger.SendMessage("Hello, World!", _participant1);
        db.SaveChanges();

        // Then: Message should be added to the Database
        var retrievedMessenger = db.MessengerEntries.First(m => m.Id == _messenger.Id);
        Assert.Single(retrievedMessenger.Messages);
    }
}
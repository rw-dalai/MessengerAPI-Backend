using MessengerAPI.Models;
using Xunit;

namespace MessengerAPI.Tests;

// NugGet Packages
//   - Microsoft.Net.Test.Sdk
//   - Xunit

public class MessengerEntryTests
{
    // Test Fixtures ----------------------------------------------------------
    //  A test fixture is a fixed state of a set of objects used as a baseline for running tests.
    private readonly User _owner;
    private readonly User _participant1;
    private readonly User _participant2;
    private readonly User _nonParticipant;
    private readonly MessengerEntry _messenger;

    // Setup Test Fixture
    //  XUnit creates a new instance of the test class for every test method.
    public MessengerEntryTests()
    {
        _owner = new User("owner@test.com", "password") { Id = 1 };
        _participant1 = new User("participant1@test.com", "password") { Id = 2 };
        _participant2 = new User("participant2@test.com", "password") { Id = 3 };
        _nonParticipant = new User("non@test.com", "password") { Id = 4 };
        _messenger = new MessengerEntry(_owner, [_participant1]);
    } 
    
    
    // Tests ----------------------------------------------------------
    //   Should_[ExpectedBehavior]_When_[TestCondition]

    // Constructor Tests
    [Fact]
    public void Should_ThrowException_When_OwnerInParticipants()
    {
        List<User> participants = [_owner, _participant1];

        Assert.Throws<MessengerException>(() => 
            new MessengerEntry(_owner, participants));
    }

    // AddParticipant Tests
    [Fact]
    public void Should_ThrowException_When_NonOwnerAddsParticipant()
    {
        Assert.Throws<MessengerException>(() => 
            _messenger.AddParticipant(_participant1.Id, _participant2));
    }

    [Fact]
    public void Should_ThrowException_When_AddingExistingParticipant()
    {
        Assert.Throws<MessengerException>(() => 
            _messenger.AddParticipant(_owner.Id, _participant1));
    }

    [Fact]
    public void Should_AddParticipant_When_OwnerAddsNewParticipant()
    {
        _messenger.AddParticipant(_owner.Id, _participant2);

        Assert.Contains(_participant2, _messenger.Participants);
    }

    // RemoveParticipant Tests
    [Fact]
    public void Should_ThrowException_When_NonOwnerRemovesParticipant()
    {
        Assert.Throws<MessengerException>(() => 
            _messenger.RemoveParticipant(_participant1.Id, _participant1));
    }

    [Fact]
    public void Should_ThrowException_When_RemovingOwner()
    {
        Assert.Throws<MessengerException>(() => 
            _messenger.RemoveParticipant(_owner.Id, _owner));
    }

    [Fact]
    public void Should_ThrowException_When_RemovingNonParticipant()
    {
        Assert.Throws<MessengerException>(() => 
            _messenger.RemoveParticipant(_owner.Id, _nonParticipant));
    }

    [Fact]
    public void Should_RemoveParticipant_When_OwnerRemovesExistingParticipant()
    {
        _messenger.RemoveParticipant(_owner.Id, _participant1);
        Assert.DoesNotContain(_participant1, _messenger.Participants);
    }

    // SendMessage Tests
    [Fact]
    public void Should_ThrowException_When_NotOwnerOrParticipantSendMessage()
    {
        Assert.Throws<MessengerException>(() =>
            _messenger.SendMessage("Test message", _nonParticipant.Id));
    }

    [Fact]
    public void Should_SendsMessage_When_OwnerSendsMessage()
    {
        _messenger.SendMessage("Test message", _owner.Id);
        Assert.Single(_messenger.Messages); 
    }
    
    [Fact]
    public void Should_SendsMessage_When_OwnerParticipantSendsMessage()
    {
        _messenger.SendMessage("Test message", _participant1.Id);
        Assert.Single(_messenger.Messages); 
    }
}
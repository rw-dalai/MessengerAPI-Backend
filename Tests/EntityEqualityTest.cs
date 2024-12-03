using MessengerAPI.Models;
using Xunit;

namespace MessengerAPI.Tests;

public class EntityEqualityTest
{
    // Test Fixtures ------------------------------------------
    private readonly User _user1;
    private readonly User _user2;  // Same as user1 (same GUID)
    private readonly User _user3;  // Different user
    
    public EntityEqualityTest()
    {
        var address = new Address("123 Street", "City", "12345");
        
        var sameGuid = Guid.NewGuid();
        _user1 = new User("test@test.com", "password", address) { Guid = sameGuid };
        _user2 = new User("test@test.com", "password", address) { Guid = sameGuid };
        _user3 = new User("other@test.com", "password", address);
    }
    
    [Fact]
    public void Should_BeEqual_When_GUIDsMatch()
    {
        // user1/user2 (same GUID)

        // Direct equality comparisons
        Assert.True(_user1 == _user2);         
        Assert.True(_user1.Equals(_user2));

        Assert.False(_user1 == _user3);        
        Assert.False(_user1.Equals(_user3));   
    }

    [Fact]
    public void Should_MaintainUniqueness_When_AddingToHashSet()
    {
        // user1/user2 (same GUID)

        // HashSet behavior
        var userSet = new HashSet<User> { _user1, _user2, _user3 };
        
        Assert.Equal(2, userSet.Count);  // Only user1/user2 (same GUID) and user3
    }

    [Fact]
    public void Should_FindMatchingEntity_When_UsingCollectionMethods()
    {
        // user1/user2 (same GUID)
        
        // List behavior
        var userList = new List<User> { _user1 };
        
        Assert.Contains(_user2, userList);  // Finds it by GUID
        Assert.DoesNotContain(_user3, userList);  // Different GUID
    }

    [Fact]
    public void Should_TreatEntitiesAsSame_When_UsingAsDictionaryKey()
    {
        // user1/user2 (same GUID)
        
        // Dictionary behavior
        var userDict = new Dictionary<User, string>
        {
            [_user1] = "First",
            [_user2] = "Second" // Overwrites "First" as they're same entity
        };

        Assert.Single(userDict);
        Assert.Equal("Second", userDict[_user1]);
        Assert.Equal("Second", userDict[_user2]);
    }

    [Fact]
    public void Should_ConsiderGUID_When_UsingLINQOperations()
    {
        // user1/user2 (same GUID)
        
        // LINQ operations
        var users = new[] { _user1, _user2, _user3 };
        
        Assert.Equal(2, users.Distinct().Count());  // user1/user2 count as one
        Assert.Contains(_user2, users); // Finds by GUID
    }

    [Fact]
    public void Should_ConsiderEntitiesEqual_When_ComparingMessengerEntries()
    {
        // user1/user2 (same GUID)

        // MessengerEntry relationships
        var messenger1 = new MessengerEntry(_user1, [_user3]);
        var messenger2 = new MessengerEntry(_user2, [_user3]);
        var participant1 = messenger1.Participants[0];
        var participant2 = messenger2.Participants[0];
        
        // Owner comparison
        Assert.Equal(messenger1.Owner, messenger2.Owner);
        
        // Participant comparison
        Assert.Equal(participant1, participant2);
    }
}
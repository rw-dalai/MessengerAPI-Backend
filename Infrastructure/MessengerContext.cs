using MessengerAPI.Models;

namespace MessengerAPI.Infrastructure;

using Microsoft.EntityFrameworkCore;

// Nuget Packages
//   - Microsoft.EntityFrameworkCore
//   - Microsoft.EntityFrameworkCore.Sqlite

public class MessengerContext(DbContextOptions<MessengerContext> options) : DbContext(options)
{
    // Tables in Database
    public DbSet<User> Users { get; set; }
    public DbSet<MessengerEntry> MessengerEntries { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User ------------------------------------------

        // (1) User owns (1) Address (1:1 Relationship)
        modelBuilder.Entity<User>()
            .OwnsOne<Address>(u => u.Address);              // User has Address
        

        // (n) Users (Participants) are in (m) MessengerEntries (n:m Relationship)
        modelBuilder.Entity<User>()
            .HasMany<MessengerEntry>()                      // User has many MessengerEntries
            .WithMany(m => m.Participants);                 // MessengerEntry has many Users
        

        // ⛔EF CORE creates this automatically ⛔️️

        // (1) User (Owner) creates (n) MessengerEntries (1:n Relationship)
        // modelBuilder.Entity<User>()
            // .HasMany<MessengerEntry>()                   // User has many MessengerEntries
            // .WithOne(m => m.Owner)                       // MessengerEntry has one Owner
            // .HasForeignKey(m => m.OwnerId);
        

        // ⛔EF CORE creates this automatically ⛔️️

        // (1) User sends (n) Messages (1:n Relationship)
        // modelBuilder.Entity<User>()
            // .HasMany<Message>()                          // User has many Messages
            // .WithOne(m => m.Sender)                      // Message has one Sender
            // .HasForeignKey(m => m.SenderId);


        // MessengerEntry ------------------------------------------
        
        // ⛔EF CORE creates this automatically ⛔️️

        // (1) MessengerEntry has (n) Messages (1:n Relationship)
        // modelBuilder.Entity<MessengerEntry>()
            // .HasMany<Message>()                          // MessengerEntry has many Messages
            // .WithOne(m => m.MessengerEntry)              // Message has one MessengerEntry
            // .HasForeignKey(m => m.MessengerEntryId);
    }
}
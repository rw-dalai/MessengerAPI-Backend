# Convention over Configuration in EF Core

- Entity Framework Core (EF Core) follows the principle of **Convention over Configuration**
- It can infer most of the model's relationships, keys, and schema details without requiring explicit configuration.

## Primary Keys (PK)
- EF Core recognizes a property named `Id` or `<EntityName>Id` as the primary key of the entity.

```csharp
public class User
{
    // This will be inferred as the primary key by EF Core.
  public int Id { get; set; }
}

public class Product
{
  // This will also be inferred as the primary key.
  public int ProductId { get; set; }
}
```

## Table and Column Naming
- EF Core uses the **class name** as the table name and **property names** as column names.
    
```csharp
public class User
{
  // This will be mapped to a column called 'Id', it's the primary key.
  public int Id { get; set; }

  // This will be mapped to a column called 'Name'.
  public string Name { get; set; }
}
```
- An entity class named `User` will be mapped to a table called `Users` by default.

## Relationships and Foreign Keys
- **Navigation Properties**: If an entity has a navigation property referring to another entity (e.g., `User` property in a `Message` class), EF Core infers that there is a relationship between these entities.
- **Foreign Key Properties**: If there is a property named `<NavigationProperty>Id` (e.g., `UserId` corresponding to a navigation property named `User`), EF Core will automatically recognize it as a **foreign key**.
    
```csharp
public class Message
{
  public int Id { get; set; }
  public string Content { get; set; }
  
  // Foreign key property
  public int UserId { get; set; }
  // Navigation property
  public User User { get; set; }
}
```
- In the `Message` entity, `UserId` is recognized as the foreign key for the `User` navigation property.

## One-to-Many Relationships
- If a class has a **collection navigation property** (e.g., `ICollection<Message>` in a `MessengerEntry`), EF Core infers that there is a one-to-many relationship between the entities.
    
```csharp
public class MessengerEntry
{
  public int Id { get; set; }
  
  // One-to-many relationship
  public ICollection<Message> Messages { get; set; } = new List<Message>();
}

public class Message
{
  public int Id { get; set; }
  public string Content { get; set; }
  
  // Foreign key property
  public int MessengerEntryId { get; set; }
  // Navigation property
  public MessengerEntry MessengerEntry { get; set; }
}
```
- `MessengerEntry` has a collection of `Messages`, which EF Core recognizes as a one-to-many relationship.

## Pluralization
- By convention, EF Core uses **pluralization** for the table name if a `DbSet` property is used.
    
```csharp
public class ApplicationDbContext : DbContext
{
  // This will create a table named 'Users'
  public DbSet<User> Users { get; set; }
}
```

## Cascade Delete
- EF Core will apply **cascading delete** rules by convention based on the relationship type:
    - For **required relationships**, it may default to `Cascade` (i.e., deleting a parent will delete its children).
- For **optional relationships**, it defaults to `Restrict`.
    
```csharp
public class Customer
{
  public int Id { get; set; }
  
  // One-to-many relationship
  public ICollection<Order> Orders { get; set; } = new List<Order>();
}

public class Order
{
  public int Id { get; set; }
  
  // Required relationship, cascade delete by default
  public int CustomerId { get; set; }
  public Customer Customer { get; set; }
}
```
- If a `Customer` is deleted, by default EF Core will delete all related `Orders` due to cascade delete.

## Optional vs. Required Relationships
- By convention, if the **foreign key property** (`<NavigationProperty>Id`) is a **nullable type** (e.g., `int?`), EF Core will infer the relationship as **optional**.
- If the foreign key property is **non-nullable** (e.g., `int`), EF Core will infer the relationship as **required**.
    
```csharp
public class Appointment
{
  public int Id { get; set; }
  public int? DoctorId { get; set; }  // Nullable, optional relationship
  public Doctor? Doctor { get; set; }
}
```
- `DoctorId` is nullable, so the relationship between `Appointment` and `Doctor` is optional.

## Shadow Properties
- EF Core can automatically create **shadow properties** (properties that are not defined in the entity class but are used by the EF model).
- For example, if no foreign key is explicitly defined but there is a navigation property, EF Core may generate a foreign key property in the database schema that is not present in your class.
    
```csharp
public class Blog
{
  public int Id { get; set; }
  public string Title { get; set; }
  public ICollection<Post> Posts { get; set; } = new List<Post>();
}

public class Post
{
  public int Id { get; set; }
  public string Content { get; set; }
  public Blog Blog { get; set; }  // Navigation property without an explicit foreign key property
}
```
- EF Core will create a shadow property to handle the foreign key between `Post` and `Blog` if no explicit `BlogId` is defined.


## Fluent API Configuration (`onModelCreating`)

Below are examples of how to configure one-to-one, one-to-many, and many-to-many relationships using the Fluent API in EF Core.

### One-to-One Relationship
```csharp
// Domain classes
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Profile Profile { get; set; }
}

public class Profile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}

// Configuration in DbContext
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<User>()
        .HasOne(u => u.Profile)  // Define a one-to-one relationship from User to Profile
        .WithOne(p => p.User)  // Define the inverse relationship from Profile to User
        .HasForeignKey<Profile>(p => p.UserId);  // Define the foreign key property in Profile
}
```
- In this example, a `User` has one `Profile`, and a `Profile` is associated with exactly one `User`.

### One-to-Many Relationship
```csharp
// Domain classes
public class Department
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Employee> Employees { get; set; }
}

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int DepartmentId { get; set; }
    public Department Department { get; set; }
}

// Configuration in DbContext
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Department>()
        .HasMany(d => d.Employees)  // The one-to-many relationship from Department to Employees
        .WithOne(e => e.Department)  // The inverse relationship from Employee to Department
        .HasForeignKey(e => e.DepartmentId);  // The foreign key property in Employee
}
```
- In this example, a `Department` can have many `Employees`, and each `Employee` belongs to one `Department`.

### Many-to-Many Relationship (With Explicit Join table)
```csharp
// Domain classes
public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    // Navigation property for many-to-many relationship
    public ICollection<StudentCourse> StudentCourses { get; set; }
}

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; }
    
    // Navigation property for many-to-many relationship
    public ICollection<StudentCourse> StudentCourses { get; set; }
}

// Join table
public class StudentCourse
{
    public int StudentId { get; set; }
    // Navigation property for Student
    public Student Student { get; set; }

    public int CourseId { get; set; }
    // Navigation property for Course
    public Course Course { get; set; }
}

// Configuration in DbContext
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<StudentCourse>()  // Configure the join table 
        .HasKey(sc => new { sc.StudentId, sc.CourseId });  // Composite key for the join table

    modelBuilder.Entity<StudentCourse>()
        .HasOne(sc => sc.Student)  // The relationship between StudentCourse and Student
        .WithMany(s => s.StudentCourses)  // A Student can have many StudentCourse relationships
        .HasForeignKey(sc => sc.StudentId);  // The foreign key in the join table

    modelBuilder.Entity<StudentCourse>()
        .HasOne(sc => sc.Course)  // The relationship between StudentCourse and Course
        .WithMany(c => c.StudentCourses)  // A Course can have many StudentCourse relationships
        .HasForeignKey(sc => sc.CourseId);  // The foreign key in the join table
}
```
- In this example, `Student` and `Course` have a many-to-many relationship facilitated by the `StudentCourse` join table.

### Many-to-Many Relationship (With Implicit Join Table)
```csharp
public class MessengerEntry
{
    public int Id { get; set; }
    public ICollection<User> Participants { get; set; } = new List<User>();
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
}

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<MessengerEntry()  // Configure MessengerEntry entity
        .HasMany(m => m.Participants) // One MessengerEntry has many Participants
        .WithMany()  // One User can be in many MessengerEntries
        .UsingEntity(  // Many-to-Many relationship
            "MessengerParticipants",  // Join table name
            l => l.HasOne(typeof(User())  // Left side: User side of relationship
                .WithMany()
                .HasForeignKey("UserId), // Foreign key for User in join table
            r => r.HasOne(typeof(MessengerEntry))  // Right side: MessengerEntry side of relationship
                .WithMany()
                .HasForeignKey("MessengerEntryId")  // Foreign key for MessengerEntry in join table
        );
}
```

## Key-Takeaway
- By following EF Core's conventions, we can minimize the need for configuration, making our code cleaner and easier to maintain. 
- Explicit configurations using Fluent API or Data Annotations are still available for non-standard situations or where more precise control is needed.

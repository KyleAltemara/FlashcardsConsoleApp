using FlashcardsConsoleApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsConsoleApp.DataAccess;

/// <summary>
/// The database context for the flashcard application.
/// Contains the <see cref="DbSet{TEntity}"/>s for the flashcards, stacks, and study sessions.
/// </summary>
public class FlashCardDbContext : DbContext
{
    public DbSet<FlashCard> FlashCards { get; set; }
    public DbSet<Stack> Stacks { get; set; }
    public DbSet<StudySession> StudySessions { get; set; }

    public FlashCardDbContext(DbContextOptions<FlashCardDbContext> options) : base(options)
    {
        // Ensure the database is created when the context is created.
        Database.EnsureCreated();
    }

    /// <summary>
    /// Configures the relationships between the entities.
    /// </summary>
    /// <param name="modelBuilder"> The <see cref="ModelBuilder"/> to use for configuration. </param>
    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        // Configure the relationship between the Stack and FlashCard entities.
        // Entity Framework Core will automatically create a foreign key in the FlashCard table
        modelBuilder.Entity<Stack>()
            // HasMany() specifies that a Stack can have many FlashCards.
            .HasMany(s => s.FlashCards)
            // WithOne() specifies that a FlashCard can have only one Stack.
            .WithOne(fc => fc.Stack)
            // HasForeignKey() specifies the foreign key property in the FlashCard entity.
            .HasForeignKey(fc => fc.StackId);
}

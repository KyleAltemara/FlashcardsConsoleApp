using FlashcardsConsoleApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsConsoleApp.DataAccess;

public class FlashCardDbContext : DbContext
{
    public DbSet<FlashCard> FlashCards { get; set; }
    public DbSet<Stack> Stacks { get; set; }
    public DbSet<StudySession> StudySessions { get; set; }

    public FlashCardDbContext(DbContextOptions<FlashCardDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<Stack>()
            .HasMany(s => s.FlashCards)
            .WithOne(fc => fc.Stack)
            .HasForeignKey(fc => fc.StackId);
}

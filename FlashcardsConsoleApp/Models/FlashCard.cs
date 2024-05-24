namespace FlashcardsConsoleApp.Models;

/// <summary>
/// Represents a flashcard, linked to a stack.
/// </summary>
public class FlashCard
{
    public int Id { get; set; }
    public int DisplayId { get; set; }
    public required string Question { get; set; }
    public required string Answer { get; set; }
    public required int StackId { get; set; }
    public Stack? Stack { get; set; }

    /// <summary>
    /// Converts the <see cref="FlashCard"/> to a DTO (Data Transfer Object).
    /// </summary>
    /// <returns>The DTO representation of the <see cref="FlashCard"/>.</returns>
    public FlashCardDTO ToDTO() => new()
    {
        DisplayId = DisplayId,
        Question = Question,
        Answer = Answer
    };
}

/// <summary>
/// Represents a DTO (Data Transfer Object) for a <see cref="FlashCard"/>.
/// </summary>
public class FlashCardDTO
{
    public required int DisplayId { get; set; }
    public required string Question { get; set; }
    public required string Answer { get; set; }
}

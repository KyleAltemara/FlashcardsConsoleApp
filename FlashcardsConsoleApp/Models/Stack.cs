namespace FlashcardsConsoleApp.Models;

/// <summary>
/// Represents a stack of flashcards.
/// </summary>
public class Stack
{
    public int Id { get; set; }
    public int DisplayId { get; set; }
    public string Name { get; set; }
    public List<FlashCard> FlashCards { get; set; }

    public Stack(string name, int displayId)
    {
        DisplayId = displayId;
        Name = name;
        FlashCards = [];
    }

    /// <summary>
    /// Converts the <see cref="Stack"/> to a DTO (Data Transfer Object).
    /// </summary>
    /// <returns>The DTO representation of the <see cref="Stack"/></returns>
    public StackDTO ToDTO() => new()
    {
        DisplayId = DisplayId,
        Name = Name,
        FlashCardCount = FlashCards.Count
    };
}

/// <summary>
/// Represents a DTO (Data Transfer Object) for a <see cref="Stack"/>.
/// </summary>
public class StackDTO
{
    public required int DisplayId { get; set; }

    public required string Name { get; set; }

    public required int FlashCardCount { get; set; }
}

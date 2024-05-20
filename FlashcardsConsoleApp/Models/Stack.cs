namespace FlashcardsConsoleApp.Models;

public class Stack
{
    public int Id { get; set; }
    public int DisplayId { get; set; }
    public string Name { get; set; }
    public List<FlashCard> FlashCards { get; set; }
    public List<StudySession> StudySessions { get; set; }

    public Stack(string name, int displayId)
    {
        DisplayId = displayId;
        Name = name;
        FlashCards = [];
        StudySessions = [];
    }

    public StackDTO ToDTO() => new()
    {
        DisplayId = DisplayId,
        Name = Name,
        FlashCardCount = FlashCards.Count
    };
}

public class StackDTO
{
    public required int DisplayId { get; set; }

    public required string Name { get; set; }

    public required int FlashCardCount { get; set; }
}

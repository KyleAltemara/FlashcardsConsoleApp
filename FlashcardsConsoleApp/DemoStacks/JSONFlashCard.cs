namespace FlashcardsConsoleApp.DemoStacks;

/// <summary>
/// Represents a flashcard in JSON format.
/// Used for deserializing JSON data.
/// </summary>
public class JSONFlashCard
{
    public string Question { get; set; }
    public string Answer { get; set; }
}
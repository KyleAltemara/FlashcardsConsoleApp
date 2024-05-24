namespace FlashcardsConsoleApp.DemoStacks;

/// <summary>
/// Represents a stack of flashcards in JSON format.
/// Used for deserializing JSON data.
/// </summary>
internal class JSONStack
{
    public string Name { get; set; }
    public List<JSONFlashCard> Cards { get; set; }
}

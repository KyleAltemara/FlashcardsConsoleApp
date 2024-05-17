namespace FlashcardsConsoleApp.Models;

public class Stack
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<FlashCard> FlashCards { get; set; }
    public List<StudySession> StudySessions { get; set; }

    public Stack(string name)
    {
        Name = name;
        FlashCards = [];
        StudySessions = [];
    }
}

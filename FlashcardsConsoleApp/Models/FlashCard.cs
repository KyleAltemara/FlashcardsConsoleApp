namespace FlashcardsConsoleApp.Models;

public class FlashCard
{
    public int Id { get; set; }
    public string Question { get; set; }
    public string Answer { get; set; }
    public int StackId { get; set; }
    public Stack Stack { get; set; }
}

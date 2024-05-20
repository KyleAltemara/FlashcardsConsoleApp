namespace FlashcardsConsoleApp.Models;

public class FlashCard
{
    public int Id { get; set; }
    public int DisplayId { get; set; }
    public required string Question { get; set; }
    public required string Answer { get; set; }
    public required int StackId { get; set; }
    public Stack? Stack { get; set; }

    public FlashCardDTO ToDTO() => new()
    {
        DisplayId = DisplayId,
        Question = Question,
        Answer = Answer
    };
}

public class FlashCardDTO
{
    public required int DisplayId { get; set; }
    public required string Question { get; set; }
    public required string Answer { get; set; }
}

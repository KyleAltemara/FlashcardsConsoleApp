namespace FlashcardsConsoleApp.Models;

public class StudySession
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int Score { get; set; }
    public Stack? Stack { get; set; }

    public StudySessionDTO ToDTO() => new()
    {
        Date = Date,
        Score = Score,
        StackName = Stack?.Name ?? null
    };
}

public class StudySessionDTO
{
    public DateTime Date { get; set; }
    public int Score { get; set; }
    public string? StackName { get; set; }
}
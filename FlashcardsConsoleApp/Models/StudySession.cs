namespace FlashcardsConsoleApp.Models;

public class StudySession
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int Correct { get; set; }
    public int Incorrect { get; set; }
    public required string StackName { get; set; }
    public int StackId { get; set; }

    public StudySessionDTO ToDTO() => new()
    {
        Date = Date,
        Correct = Correct,
        Incorrect = Incorrect,
        StackName = StackName,
    };
}

public class StudySessionDTO
{
    public DateTime Date { get; set; }
    public int Correct { get; set; }
    public int Incorrect { get; set; }
    public required string StackName { get; set; }
}
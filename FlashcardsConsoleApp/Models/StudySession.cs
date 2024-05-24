namespace FlashcardsConsoleApp.Models;

/// <summary>
/// Stores information about a study session.
/// </summary>
public class StudySession
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int Correct { get; set; }
    public int Incorrect { get; set; }
    public required string StackName { get; set; }
    public int StackId { get; set; }

    /// <summary>
    /// Converts the <see cref="StudySession"/> to a DTO (Data Transfer Object).
    /// </summary>
    /// <returns>The DTO representation of the <see cref="StudySession"/>.</returns>
    public StudySessionDTO ToDTO() => new()
    {
        Date = Date,
        Correct = Correct,
        Incorrect = Incorrect,
        StackName = StackName,
    };
}

/// <summary>
/// Represents a DTO (Data Transfer Object) for a <see cref="StudySession"/>.
/// </summary>
public class StudySessionDTO
{
    public DateTime Date { get; set; }
    public int Correct { get; set; }
    public int Incorrect { get; set; }
    public required string StackName { get; set; }
}
using FlashcardsConsoleApp.Models;

namespace FlashcardsConsoleApp.DataAccess.Repositories;

/// <summary>
/// Represents a study session repository.
/// Used for storing and retrieving study session data.
/// </summary>
public interface IStudySessionRepository
{
    /// <summary>
    /// Gets all study sessions.
    /// </summary>
    /// <returns> A list of study sessions. </returns>
    Task<IList<StudySession>> GetAllAsync();

    /// <summary>
    /// Adds a study session.
    /// </summary>
    /// <param name="studySession"> The study session to add. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    Task AddAsync(StudySession studySession);

    /// <summary>
    /// Gets study sessions by stack ID.
    /// </summary>
    /// <param name="stackId"> The ID of the stack to get study sessions for. </param>
    /// <returns> A list of study sessions. </returns>
    Task<List<StudySession>> GetByStackIdAsync(int stackId);

    /// <summary>
    /// Deletes study sessions by stack ID.
    /// </summary>
    /// <param name="stackId"> The ID of the stack to delete study sessions for. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    Task DeleteAsync(int stackId);
}

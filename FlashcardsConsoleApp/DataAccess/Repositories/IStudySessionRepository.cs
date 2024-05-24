using FlashcardsConsoleApp.Models;

namespace FlashcardsConsoleApp.DataAccess.Repositories;

public interface IStudySessionRepository
{
    Task<IList<StudySession>> GetAllAsync();
    Task AddAsync(StudySession studySession);
    Task<List<StudySession>> GetByStackIdAsync(int stackId);
    Task DeleteAsync(int stackId);
}

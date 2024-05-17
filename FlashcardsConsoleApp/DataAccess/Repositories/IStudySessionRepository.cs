using FlashcardsConsoleApp.Models;

namespace FlashcardsConsoleApp.DataAccess.Repositories;

public interface IStudySessionRepository
{
    Task<IEnumerable<StudySession>> GetAllAsync();
    Task<StudySession> GetByIdAsync(int id);
    Task AddAsync(StudySession studySession);
    Task UpdateAsync(StudySession studySession);
    Task DeleteAsync(int id);
}

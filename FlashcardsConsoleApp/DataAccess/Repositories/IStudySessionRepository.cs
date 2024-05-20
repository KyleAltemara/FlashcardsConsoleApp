using FlashcardsConsoleApp.Models;

namespace FlashcardsConsoleApp.DataAccess.Repositories;

public interface IStudySessionRepository
{
    Task<IList<StudySession>> GetAllAsync();
    Task<StudySession?> GetByIdAsync(int id);
    Task AddAsync(StudySession studySession);
    Task UpdateAsync(StudySession studySession);
    Task DeleteAsync(int id);
    Task DeleteAsync(List<StudySession> studySessions);
}

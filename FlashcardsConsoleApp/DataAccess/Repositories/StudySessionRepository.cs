using FlashcardsConsoleApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsConsoleApp.DataAccess.Repositories;

/// <summary>
/// Represents a study session repository.
/// see <see cref="IStudySessionRepository"/>.
/// </summary>
public class StudySessionRepository : IStudySessionRepository
{
    private readonly FlashCardDbContext _context;

    public StudySessionRepository(FlashCardDbContext context)
    {
        _context = context;
    }

    public async Task<IList<StudySession>> GetAllAsync() => await _context.StudySessions.ToListAsync();

    public async Task AddAsync(StudySession studySession)
    {
        _context.StudySessions.Add(studySession);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int stackId)
    {
        var studySessions = await GetByStackIdAsync(stackId);
        _context.StudySessions.RemoveRange(studySessions);
        await _context.SaveChangesAsync();
    }

    public async Task<List<StudySession>> GetByStackIdAsync(int stackId) => await _context.StudySessions.Where(ss => ss.StackId == stackId).ToListAsync();
}

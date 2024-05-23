using FlashcardsConsoleApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsConsoleApp.DataAccess.Repositories;

public class StudySessionRepository : IStudySessionRepository
{
    private readonly FlashCardDbContext _context;

    public StudySessionRepository(FlashCardDbContext context)
    {
        _context = context;
    }

    public async Task<IList<StudySession>> GetAllAsync() => await _context.StudySessions.ToListAsync();

    public async Task<StudySession?> GetByIdAsync(int id) => await _context.StudySessions.FindAsync(id);

    public async Task AddAsync(StudySession studySession)
    {
        _context.StudySessions.Add(studySession);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(StudySession studySession)
    {
        _context.StudySessions.Update(studySession);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var studySession = await _context.StudySessions.FindAsync(id);
        if (studySession != null)
        {
            _context.StudySessions.Remove(studySession);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(List<StudySession> studySessions)
    {
        _context.StudySessions.RemoveRange(studySessions);
        await _context.SaveChangesAsync();
    }

    public async Task<List<StudySession>> GetByStackIdAsync(int stackId) => await _context.StudySessions.Where(ss => ss.StackId == stackId).ToListAsync();
}

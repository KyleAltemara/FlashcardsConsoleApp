using FlashcardsConsoleApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsConsoleApp.DataAccess.Repositories;

public class StackRepository : IStackRepository
{
    private readonly FlashCardDbContext _context;
    private readonly FlashCardRepository _flashCardRepository;
    private readonly StudySessionRepository _studySessionsRepository;

    public StackRepository(FlashCardDbContext context)
    {
        _context = context;
        _flashCardRepository = new FlashCardRepository(_context);
        _studySessionsRepository = new StudySessionRepository(_context);
    }

    public async Task<IList<Stack>> GetAllAsync() => await _context.Stacks.Include(s => s.FlashCards).ToListAsync();

    public async Task<Stack?> GetByIdAsync(int id) => await _context.Stacks.Include(s => s.FlashCards).FirstOrDefaultAsync(s => s.Id == id);

    public async Task<Stack?> GetByNameAsync(string name) => await _context.Stacks.Include(s => s.FlashCards).FirstOrDefaultAsync(s => s.Name == name);

    public async Task AddAsync(Stack stack)
    {
        _context.Stacks.Add(stack);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Stack stack)
    {
        _context.Stacks.Update(stack);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var stack = await _context.Stacks.FindAsync(id);
        if (stack != null)
        {
            await _flashCardRepository.DeleteAsync(stack.FlashCards);
            await _studySessionsRepository.DeleteAsync(stack.StudySessions);
            _context.Stacks.Remove(stack);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetMaxIdAsync() => await _context.Stacks.AnyAsync() ? await _context.Stacks.MaxAsync(s => s.Id) : -1;
}

using FlashcardsConsoleApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsConsoleApp.DataAccess.Repositories;

public class StackRepository : IStackRepository
{
    private readonly FlashCardDbContext _context;
    private readonly IFlashCardRepository _flashCardRepository;
    private readonly IStudySessionRepository _studySessionsRepository;

    public StackRepository(FlashCardDbContext context, IFlashCardRepository flashCardRepository, IStudySessionRepository studySessionsRepository)
    {
        _context = context;
        _flashCardRepository = flashCardRepository;
        _studySessionsRepository = studySessionsRepository;
    }

    public async Task<IList<Stack>> GetAllAsync() => await _context.Stacks.Include(s => s.FlashCards).ToListAsync();

    public async Task<Stack?> GetByIdAsync(int id) => await _context.Stacks.FindAsync(id);

    public async Task<Stack?> GetByNameAsync(string name) => await _context.Stacks.FirstOrDefaultAsync(s => s.Name == name);

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

            // Renumber DisplayId in the remaining stacks
            var remainingStacks = await _context.Stacks.ToListAsync();
            foreach (var remainingStack in remainingStacks)
            {
                if (remainingStack.DisplayId > stack.DisplayId)
                {
                    remainingStack.DisplayId--;
                }
            }

            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetMaxIdAsync() => await _context.Stacks.AnyAsync() ? await _context.Stacks.MaxAsync(s => s.Id) : 0;

    public async Task<int> GetStackIdFromDisplayId(object displayId) => await _context.Stacks.Where(s => s.DisplayId == (int)displayId).Select(s => s.Id).SingleOrDefaultAsync();
}

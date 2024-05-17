using FlashcardsConsoleApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsConsoleApp.DataAccess.Repositories;

public class FlashCardRepository : IFlashCardRepository
{
    private readonly FlashCardDbContext _context;

    public FlashCardRepository(FlashCardDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FlashCard>> GetAllAsync() => await _context.FlashCards.ToListAsync();

    public async Task<FlashCard?> GetByIdAsync(int id) => await _context.FlashCards.FindAsync(id);

    public async Task AddAsync(FlashCard flashCard)
    {
        _context.FlashCards.Add(flashCard);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(FlashCard flashCard)
    {
        _context.FlashCards.Update(flashCard);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var flashCard = await _context.FlashCards.FindAsync(id);
        if (flashCard != null)
        {
            _context.FlashCards.Remove(flashCard);
            await _context.SaveChangesAsync();
        }
    }
}

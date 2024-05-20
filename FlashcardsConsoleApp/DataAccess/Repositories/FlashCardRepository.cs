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

    public async Task<IList<FlashCard>> GetAllAsync() => await _context.FlashCards.ToListAsync();

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
            var remainingFlashCards = await _context.FlashCards.OrderBy(fc => fc.DisplayId).ToListAsync();
            for (int i = 0; i < remainingFlashCards.Count; i++)
            {
                remainingFlashCards[i].DisplayId = i + 1;
            }

            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(List<FlashCard> flashCards)
    {
        _context.FlashCards.RemoveRange(flashCards);
        var remainingFlashCards = await _context.FlashCards.OrderBy(fc => fc.DisplayId).ToListAsync();
        for (int i = 0; i < remainingFlashCards.Count; i++)
        {
            remainingFlashCards[i].DisplayId = i + 1;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<FlashCard>> GetByStackIdAsync(int stackId) => await _context.FlashCards.Where(fc => fc.StackId == stackId).ToListAsync();

    public async Task<int> GetMaxIdAsync() => await _context.FlashCards.AnyAsync() ? await _context.FlashCards.MaxAsync(fc => fc.Id) : 0;

    public async Task<int> GetFlashCardIdFromDisplayId(int displayId) => await _context.FlashCards.Where(fc => fc.DisplayId == displayId).Select(fc => fc.Id).SingleOrDefaultAsync();
}

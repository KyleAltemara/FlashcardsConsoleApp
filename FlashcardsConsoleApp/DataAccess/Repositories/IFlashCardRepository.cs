using FlashcardsConsoleApp.Models;

namespace FlashcardsConsoleApp.DataAccess.Repositories;

public interface IFlashCardRepository
{
    Task<IList<FlashCard>> GetAllAsync();
    Task<FlashCard?> GetByIdAsync(int id);
    Task AddAsync(FlashCard flashCard);
    Task UpdateAsync(FlashCard flashCard);
    Task DeleteAsync(int id);
    Task DeleteAsync(List<FlashCard> flashCards);
    Task<IEnumerable<FlashCard>> GetByStackIdAsync(int stackId);
    Task<int> GetMaxIdAsync();
    Task<int> GetFlashCardIdFromDisplayId(int displayId);
}

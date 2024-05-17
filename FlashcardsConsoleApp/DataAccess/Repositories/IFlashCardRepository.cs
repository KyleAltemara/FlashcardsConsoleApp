using FlashcardsConsoleApp.Models;

namespace FlashcardsConsoleApp.DataAccess.Repositories;

public interface IFlashCardRepository
{
    Task<IEnumerable<FlashCard>> GetAllAsync();
    Task<FlashCard?> GetByIdAsync(int id);
    Task AddAsync(FlashCard flashCard);
    Task UpdateAsync(FlashCard flashCard);
    Task DeleteAsync(int id);
}

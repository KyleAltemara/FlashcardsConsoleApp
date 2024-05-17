using FlashcardsConsoleApp.Models;

namespace FlashcardsConsoleApp.DataAccess.Repositories;

public interface IStackRepository
{
    Task<IEnumerable<Stack>> GetAllAsync();
    Task<Stack?> GetByIdAsync(int id);
    Task<Stack?> GetByNameAsync(string name);
    Task AddAsync(Stack stack);
    Task UpdateAsync(Stack stack);
    Task DeleteAsync(int id);
    Task<int> GetMaxIdAsync();
}

using FlashcardsConsoleApp.Models;

namespace FlashcardsConsoleApp.Services;

public interface IStackService
{
    Task<IEnumerable<Stack>> GetAllStacksAsync();
    Task<Stack?> GetStackByIdAsync(int id);
    Task<Stack?> GetStackByNameAsync(string name);

    Task<bool> AddStackAsync(string name);
    Task RenameStackAsync(int id, string name);
    Task DeleteStackAsync(int id);
    Task AddFlashCardToStackAsync(int stackId, string question, string answer);
}

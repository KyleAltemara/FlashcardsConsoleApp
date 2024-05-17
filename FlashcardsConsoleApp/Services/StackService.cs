using FlashcardsConsoleApp.DataAccess.Repositories;
using FlashcardsConsoleApp.Models;

namespace FlashcardsConsoleApp.Services;

public class StackService : IStackService
{
    private readonly IStackRepository _stackRepository;
    private readonly IFlashCardRepository _flashCardRepository;

    public StackService(IStackRepository stackRepository, IFlashCardRepository flashCardRepository)
    {
        _stackRepository = stackRepository;
        _flashCardRepository = flashCardRepository;
    }

    public async Task<IEnumerable<Stack>> GetAllStacksAsync() => await _stackRepository.GetAllAsync();

    public async Task<Stack?> GetStackByIdAsync(int id) => await _stackRepository.GetByIdAsync(id);

    public async Task<Stack?> GetStackByNameAsync(string name) => await _stackRepository.GetByNameAsync(name);

    public async Task<bool> AddStackAsync(string name)
    {
        var existingStack = await _stackRepository.GetByNameAsync(name);
        if (existingStack != null)
        {
            // Handle the case when the stack name is not unique
            return false;
        }

        var id = await _stackRepository.GetMaxIdAsync() + 1;
        var newStack = new Stack(name);
        await _stackRepository.AddAsync(newStack);
        return true;
    }

    public async Task RenameStackAsync(int id, string name)
    {
        var stack = await _stackRepository.GetByIdAsync(id);
        if (stack != null)
        {
            stack.Name = name;
            await _stackRepository.UpdateAsync(stack);
        }
    }

    public async Task DeleteStackAsync(int id) => await _stackRepository.DeleteAsync(id);

    public async Task AddFlashCardToStackAsync(int stackId, string question, string answer)
    {
        var newFlashCard = new FlashCard
        {
            Question = question,
            Answer = answer,
            StackId = stackId
        };
        await _flashCardRepository.AddAsync(newFlashCard);
    }
}

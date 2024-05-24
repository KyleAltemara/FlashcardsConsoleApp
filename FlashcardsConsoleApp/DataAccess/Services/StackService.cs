using FlashcardsConsoleApp.DataAccess.Repositories;
using FlashcardsConsoleApp.Models;

namespace FlashcardsConsoleApp.DataAccess.Services;

/// <summary>
/// Service class for managing stacks, flashcards, and study sessions.
/// See <see cref="IStackService"/> for the interface."/>
/// </summary>
public class StackService : IStackService
{
    private readonly IStackRepository _stackRepository;
    private readonly IFlashCardRepository _flashCardRepository;
    private readonly IStudySessionRepository _studySessionRepository;

    public StackService(IStackRepository stackRepository, IFlashCardRepository flashCardRepository, IStudySessionRepository studySessionRepository)
    {
        _stackRepository = stackRepository;
        _flashCardRepository = flashCardRepository;
        _studySessionRepository = studySessionRepository;
    }

    public async Task<IEnumerable<StackDTO>> GetAllStacksAsync() => (await _stackRepository.GetAllAsync()).Select(s => s.ToDTO());

    public async Task<StackDTO?> GetStackByIdAsync(int id) => (await _stackRepository.GetByIdAsync(id))?.ToDTO();

    public async Task<StackDTO?> GetStackByNameAsync(string name) => (await _stackRepository.GetByNameAsync(name))?.ToDTO();

    public async Task<bool> AddStackAsync(string name)
    {
        // Handle the case when the stack name is not unique
        var existingStack = await _stackRepository.GetByNameAsync(name);
        if (existingStack != null)
        {
            return false;
        }

        var newStackDisplayId = await _stackRepository.GetMaxIdAsync() + 1;
        var newStack = new Stack(name, newStackDisplayId);
        await _stackRepository.AddAsync(newStack);
        return true;
    }

    public async Task<bool> RenameStackAsync(int id, string name)
    {
        var existingStack = await _stackRepository.GetByNameAsync(name);
        if (existingStack != null)
        {
            return false;
        }

        var stack = await _stackRepository.GetByIdAsync(id);
        if (stack != null)
        {
            stack.Name = name;
            await _stackRepository.UpdateAsync(stack);
            return true;
        }

        return false;
    }

    public async Task DeleteStackAsync(int id) => await _stackRepository.DeleteAsync(id);

    public async Task AddFlashCardToStackAsync(int stackId, string question, string answer)
    {
        var newFlashCard = new FlashCard
        {
            Question = question,
            Answer = answer,
            StackId = stackId,
            DisplayId = await _flashCardRepository.GetMaxIdAsync() + 1
        };
        await _flashCardRepository.AddAsync(newFlashCard);
    }

    public async Task<IEnumerable<FlashCardDTO>> GetFlashCardsByStackId(int viewStackId) => (await _flashCardRepository.GetByStackIdAsync(viewStackId)).Select(fc => fc.ToDTO());

    public async Task<int> GetStackIdFromDisplayId(object displayId) => await _stackRepository.GetStackIdFromDisplayId(displayId);

    public async Task<int> GetFlashCardIdFromDisplayId(int displayId) => await _flashCardRepository.GetFlashCardIdFromDisplayId(displayId);

    public async Task UpdateFlashCardAsync(int flashcardId, string newQuestion, string newAnswer)
    {
        var flashCard = await _flashCardRepository.GetByIdAsync(flashcardId);
        if (flashCard != null)
        {
            flashCard.Question = newQuestion;
            flashCard.Answer = newAnswer;
            await _flashCardRepository.UpdateAsync(flashCard);
        }
    }

    public async Task DeleteFlashCardAsync(int flashcardId) => await _flashCardRepository.DeleteAsync(flashcardId);

    public async Task AddStudySession(DateTime now, int correctCount, int incorrectCount, string name, int stackId)
    {
        var newStudySession = new StudySession
        {
            Date = now,
            Correct = correctCount,
            Incorrect = incorrectCount,
            StackName = name,
            StackId = stackId
        };

        await _studySessionRepository.AddAsync(newStudySession);
    }

    public async Task<IEnumerable<StudySessionDTO>> GetStudySessionsByStackId(int stackId) => (await _studySessionRepository.GetByStackIdAsync(stackId)).Select(ss => ss.ToDTO());

    public async Task<IEnumerable<StudySessionDTO>> GetAllStudySessionData() => (await _studySessionRepository.GetAllAsync()).Select(ss => ss.ToDTO());
}

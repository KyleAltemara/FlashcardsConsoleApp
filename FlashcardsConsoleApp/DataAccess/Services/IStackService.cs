using FlashcardsConsoleApp.Models;

namespace FlashcardsConsoleApp.DataAccess.Services;

public interface IStackService
{
    Task<IEnumerable<StackDTO>> GetAllStacksAsync();
    Task<StackDTO?> GetStackByIdAsync(int id);
    Task<StackDTO?> GetStackByNameAsync(string name);
    Task<bool> AddStackAsync(string name);
    Task<bool> RenameStackAsync(int id, string name);
    Task DeleteStackAsync(int id);
    Task AddFlashCardToStackAsync(int stackId, string question, string answer);
    Task<IEnumerable<FlashCardDTO>> GetFlashCardsByStackId(int viewStackId);
    Task<int> GetStackIdFromDisplayId(object displayId);
    Task<int> GetFlashCardIdFromDisplayId(int displayId);
    Task UpdateFlashCardAsync(int flashcardId, string newQuestion, string newAnswer);
    Task DeleteFlashCardAsync(int flashcardId);
    Task AddStudySession(DateTime now, int correctCount, int incorrectCount, string stackname, int stackId);
    Task<IEnumerable<StudySessionDTO>> GetStudySessionsByStackId(int stackId);
    Task<IEnumerable<StudySessionDTO>> GetAllStudySessionData();
}

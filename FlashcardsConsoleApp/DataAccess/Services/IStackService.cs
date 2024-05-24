using FlashcardsConsoleApp.Models;

namespace FlashcardsConsoleApp.DataAccess.Services;

/// <summary>
/// Interface for the StackService.
/// Contains methods for interacting with the stack repository, flashcard repository, and study session repository.
/// </summary>
public interface IStackService
{
    /// <summary>
    /// Gets all stacks asynchronously.
    /// </summary>
    /// <returns> A collection of <see cref="StackDTO"/> objects representing the stacks.</returns>
    Task<IEnumerable<StackDTO>> GetAllStacksAsync();

    /// <summary>
    /// Gets a stack by its ID asynchronously.
    /// </summary>
    /// <param name="id"> The ID of the stack to get.</param>
    /// <returns> The <see cref="StackDTO"/> object representing the stack, or null if the stack is not found.</returns>
    Task<StackDTO?> GetStackByIdAsync(int id);

    /// <summary>
    /// Gets a stack by its name asynchronously.
    /// </summary>
    /// <param name="name"> The name of the stack to get.</param>
    /// <returns> The <see cref="StackDTO"/> object representing the stack, or null if the stack is not found.</returns>
    Task<StackDTO?> GetStackByNameAsync(string name);

    /// <summary>
    /// Adds a new stack asynchronously.
    /// </summary>
    /// <param name="name"> The name of the new stack.</param>
    /// <returns> True if the stack was added successfully, false if the stack name is not unique.</returns>
    Task<bool> AddStackAsync(string name);

    /// <summary>
    /// Renames a stack asynchronously.
    /// </summary>
    /// <param name="id"> The ID of the stack to rename.</param>
    /// <param name="name"> The new name of the stack.</param>
    /// <returns> True if the stack was renamed successfully, false if the new name is not unique or the stack is not found.</returns>
    Task<bool> RenameStackAsync(int id, string name);

    /// <summary>
    /// Deletes a stack asynchronously.
    /// </summary>
    /// <param name="id"> The ID of the stack to delete.</param>
    /// <returns> A task that represents the asynchronous operation.</returns>
    Task DeleteStackAsync(int id);

    /// <summary>
    /// Adds a flashcard to a stack asynchronously.
    /// </summary>
    /// <param name="stackId"> The ID of the stack to add the flashcard to.</param>
    /// <param name="question"> The question of the flashcard.</param>
    /// <param name="answer"> The answer of the flashcard.</param>
    /// <returns> A task that represents the asynchronous operation.</returns>
    Task AddFlashCardToStackAsync(int stackId, string question, string answer);

    /// <summary>
    /// Gets all flashcards in a stack asynchronously.
    /// </summary>
    /// <param name="viewStackId"> The ID of the stack to get the flashcards from.</param>
    /// <returns> A collection of <see cref="FlashCardDTO"/> objects representing the flashcards.</returns>
    Task<IEnumerable<FlashCardDTO>> GetFlashCardsByStackId(int viewStackId);

    /// <summary>
    /// Gets the stack ID from a display ID asynchronously.
    /// </summary>
    /// <param name="displayId"> The display ID of the stack to get the ID of.</param>
    /// <returns> The ID of the stack, or -1 if the stack is not found.</returns>
    Task<int> GetStackIdFromDisplayId(object displayId);

    /// <summary>
    /// Gets the flashcard ID from a display ID asynchronously.
    /// </summary>
    /// <param name="displayId"> The display ID of the flashcard to get the ID of.</param>
    /// <returns> The ID of the flashcard, or -1 if the flashcard is not found.</returns>
    Task<int> GetFlashCardIdFromDisplayId(int displayId);

    /// <summary>
    /// Updates a flashcard asynchronously.
    /// </summary>
    /// <param name="flashcardId"> The ID of the flashcard to update.</param>
    /// <param name="newQuestion"> The new question of the flashcard.</param>
    /// <param name="newAnswer"> The new answer of the flashcard.</param>
    /// <returns> A task that represents the asynchronous operation.</returns>
    Task UpdateFlashCardAsync(int flashcardId, string newQuestion, string newAnswer);

    /// <summary>
    /// Deletes a flashcard asynchronously.
    /// </summary>
    /// <param name="flashcardId"> The ID of the flashcard to delete.</param>
    /// <returns> A task that represents the asynchronous operation.</returns>
    Task DeleteFlashCardAsync(int flashcardId);

    /// <summary>
    /// Adds a study session asynchronously.
    /// </summary>
    /// <param name="now"> The date and time of the study session.</param>
    /// <param name="correctCount"> The number of correct answers in the study session.</param>
    /// <param name="incorrectCount"> The number of incorrect answers in the study session.</param>
    /// <param name="stackname"> The name of the stack studied.</param>
    /// <param name="stackId"> The ID of the stack studied.</param>
    /// <returns> A task that represents the asynchronous operation.</returns>
    Task AddStudySession(DateTime now, int correctCount, int incorrectCount, string stackname, int stackId);

    /// <summary>
    /// Gets all study sessions by stack ID asynchronously.
    /// </summary>
    /// <param name="stackId"> The ID of the stack to get the study sessions from.</param>
    /// <returns> A collection of <see cref="StudySessionDTO"/> objects representing the study sessions.</returns>
    Task<IEnumerable<StudySessionDTO>> GetStudySessionsByStackId(int stackId);

    /// <summary>
    /// Gets all study session data asynchronously.
    /// </summary>
    /// <returns> A collection of <see cref="StudySessionDTO"/> objects representing all study sessions.</returns>
    Task<IEnumerable<StudySessionDTO>> GetAllStudySessionData();
}

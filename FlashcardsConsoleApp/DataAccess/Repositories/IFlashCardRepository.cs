using FlashcardsConsoleApp.Models;

namespace FlashcardsConsoleApp.DataAccess.Repositories;

/// <summary>
/// Represents a flashcard repository.
/// Used for storing and retrieving flashcard data.
/// </summary>
public interface IFlashCardRepository
{
    /// <summary>
    /// Gets all flashcards.
    /// </summary>
    /// <returns> A list of <see cref="FlashCard"/>. </returns>
    Task<IList<FlashCard>> GetAllAsync();

    /// <summary>
    /// Gets a flashcard by ID.
    /// </summary>
    /// <param name="id"> The ID of the flashcard to get. </param>
    /// <returns> The <see cref="FlashCard"/> object, or null if the flashcard is not found. </returns>
    Task<FlashCard?> GetByIdAsync(int id);

    /// <summary>
    /// Adds a flashcard.
    /// </summary>
    /// <param name="flashCard"> The flashcard to add. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    Task AddAsync(FlashCard flashCard);

    /// <summary>
    /// Updates a flashcard.
    /// </summary>
    /// <param name="flashCard"> The flashcard to update. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    Task UpdateAsync(FlashCard flashCard);

    /// <summary>
    /// Deletes a flashcard.
    /// </summary>
    /// <param name="id"> The ID of the flashcard to delete. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    Task DeleteAsync(int id);

    /// <summary>
    /// Deletes a list of flashcards.
    /// </summary>
    /// <param name="flashCards"> The list of flashcards to delete. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    Task DeleteAsync(List<FlashCard> flashCards);

    /// <summary>
    /// Gets flashcards by stack ID.
    /// </summary>
    /// <param name="stackId"> The ID of the stack to get flashcards for. </param>
    /// <returns> A list of <see cref="FlashCard"/>. </returns>"/>
    Task<IEnumerable<FlashCard>> GetByStackIdAsync(int stackId);

    /// <summary>
    /// Gets the maximum ID of the flashcards.
    /// </summary>
    /// <returns> The maximum ID of the flashcards. </returns>
    Task<int> GetMaxIdAsync();

    /// <summary>
    /// Gets the flashcard ID from the display ID.
    /// </summary>
    /// <param name="displayId"> The display ID of the flashcard. </param>
    /// <returns> The ID of the flashcard. </returns>
    Task<int> GetFlashCardIdFromDisplayId(int displayId);
}

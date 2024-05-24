using FlashcardsConsoleApp.Models;

namespace FlashcardsConsoleApp.DataAccess.Repositories;

/// <summary>
/// Represents a stack repository.
/// Used for storing and retrieving stack data.
/// </summary>
public interface IStackRepository
{
    /// <summary>
    /// Gets all stacks.
    /// </summary>
    /// <returns> A list of <see cref="Stack"/>. </returns>
    Task<IList<Stack>> GetAllAsync();

    /// <summary>
    /// Gets a stack by ID.
    /// </summary>
    /// <param name="id"> The ID of the stack to get. </param>
    /// <returns> The <see cref="Stack"/> object, or null if the stack is not found. </returns>
    Task<Stack?> GetByIdAsync(int id);

    /// <summary>
    /// Gets a stack by name.
    /// </summary>
    /// <param name="name"> The name of the stack to get. </param>
    /// <returns> The <see cref="Stack"/> object, or null if the stack is not found. </returns>
    Task<Stack?> GetByNameAsync(string name);

    /// <summary>
    /// Adds a stack.
    /// </summary>
    /// <param name="stack"> The stack to add. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    Task AddAsync(Stack stack);

    /// <summary>
    /// Updates a stack.
    /// </summary>
    /// <param name="stack"> The stack to update. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    Task UpdateAsync(Stack stack);

    /// <summary>
    /// Deletes a stack.
    /// </summary>
    /// <param name="id"> The ID of the stack to delete. </param>
    /// <returns> A task that represents the asynchronous operation. </returns>
    Task DeleteAsync(int id);

    /// <summary>
    /// Gets the maximum ID of the stacks.
    /// </summary>
    /// <returns> The maximum ID of the stacks. </returns>
    Task<int> GetMaxIdAsync();

    /// <summary>
    /// Gets the stack ID from the display ID.
    /// </summary>
    /// <param name="displayId"> The display ID of the stack. </param>
    /// <returns> The ID of the stack. </returns>
    Task<int> GetStackIdFromDisplayId(object displayId);
}

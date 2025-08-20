using EtabsExtensions.Core.Models;

namespace EtabsExtensions.Core.Services;

/// <summary>
/// Service interface for managing Todo items
/// </summary>
public interface ITodoService
{
    /// <summary>
    /// Gets all todo items ordered by creation date (newest first)
    /// </summary>
    /// <returns>Collection of all todo items</returns>
    Task<IEnumerable<TodoItem>> GetAllAsync();

    /// <summary>
    /// Gets a todo item by its ID
    /// </summary>
    /// <param name="id">The todo item ID</param>
    /// <returns>The todo item if found, null otherwise</returns>
    Task<TodoItem?> GetByIdAsync(int id);

    /// <summary>
    /// Creates a new todo item
    /// </summary>
    /// <param name="todoItem">The todo item to create</param>
    /// <returns>The created todo item with assigned ID</returns>
    Task<TodoItem> CreateAsync(TodoItem todoItem);

    /// <summary>
    /// Updates an existing todo item
    /// </summary>
    /// <param name="todoItem">The todo item to update</param>
    /// <returns>The updated todo item</returns>
    Task<TodoItem> UpdateAsync(TodoItem todoItem);

    /// <summary>
    /// Deletes a todo item by its ID
    /// </summary>
    /// <param name="id">The todo item ID to delete</param>
    /// <returns>True if deleted successfully, false otherwise</returns>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Gets the total count of all todo items
    /// </summary>
    /// <returns>Total number of todo items</returns>
    Task<int> GetTotalCountAsync();

    /// <summary>
    /// Gets the count of completed todo items
    /// </summary>
    /// <returns>Number of completed todo items</returns>
    Task<int> GetCompletedCountAsync();

    /// <summary>
    /// Gets the count of pending (not completed) todo items
    /// </summary>
    /// <returns>Number of pending todo items</returns>
    Task<int> GetPendingCountAsync();

    /// <summary>
    /// Gets the most recent todo items
    /// </summary>
    /// <param name="count">Number of recent items to retrieve (default: 10)</param>
    /// <returns>Collection of recent todo items</returns>
    Task<IEnumerable<TodoItem>> GetRecentAsync(int count = 10);

    /// <summary>
    /// Bulk updates the completion status of multiple todo items
    /// </summary>
    /// <param name="todoIds">Collection of todo item IDs to update</param>
    /// <param name="isCompleted">New completion status</param>
    /// <returns>True if any items were updated, false otherwise</returns>
    Task<bool> BulkUpdateCompletionStatusAsync(IEnumerable<int> todoIds, bool isCompleted);
}
using EtabsExtensions.Core.Models;
using EtabsExtensions.Core.Services;
using EtabsExtensions.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace EtabsExtensions.Infrastructure.Services;

public class TodoService : ITodoService
{
    private readonly TodoDbContext _context;
    private readonly ILogger<TodoService> _logger;

    public TodoService(TodoDbContext context, ILogger<TodoService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<TodoItem>> GetAllAsync()
    {
        try
        {
            _logger.LogDebug("Retrieving all todo items");

            var todos = await _context.TodoItems
                .OrderByDescending(t => t.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} todo items", todos.Count);
            return todos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving todo items");
            throw new InvalidOperationException("Failed to retrieve todo items", ex);
        }
    }

    public async Task<TodoItem?> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid todo item ID provided: {Id}", id);
            return null;
        }

        try
        {
            _logger.LogDebug("Retrieving todo item with ID: {Id}", id);

            var todo = await _context.TodoItems
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (todo == null)
            {
                _logger.LogWarning("Todo item with ID {Id} not found", id);
            }
            else
            {
                _logger.LogDebug("Successfully retrieved todo item with ID {Id}", id);
            }

            return todo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving todo item with ID: {Id}", id);
            throw new InvalidOperationException($"Failed to retrieve todo item with ID {id}", ex);
        }
    }

    public async Task<TodoItem> CreateAsync(TodoItem todoItem)
    {
        ArgumentNullException.ThrowIfNull(todoItem);

        if (string.IsNullOrWhiteSpace(todoItem.Title))
        {
            throw new ArgumentException("Todo title cannot be empty", nameof(todoItem));
        }

        try
        {
            _logger.LogDebug("Creating new todo item: {Title}", todoItem.Title);

            // Ensure proper timestamps and state
            todoItem.CreatedAt = DateTime.UtcNow;
            todoItem.CompletedAt = null; // New todos are not completed
            todoItem.IsCompleted = false;

            // Sanitize input
            todoItem.Title = todoItem.Title.Trim();
            todoItem.Description = string.IsNullOrWhiteSpace(todoItem.Description)
                ? string.Empty
                : todoItem.Description.Trim();

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully created todo item with ID: {Id}", todoItem.Id);
            return todoItem;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating todo item: {Title}", todoItem.Title);
            throw new InvalidOperationException("Failed to create todo item", ex);
        }
    }

    public async Task<TodoItem> UpdateAsync(TodoItem todoItem)
    {
        ArgumentNullException.ThrowIfNull(todoItem);

        if (todoItem.Id <= 0)
        {
            throw new ArgumentException("Invalid todo item ID", nameof(todoItem));
        }

        if (string.IsNullOrWhiteSpace(todoItem.Title))
        {
            throw new ArgumentException("Todo title cannot be empty", nameof(todoItem));
        }

        try
        {
            _logger.LogDebug("Updating todo item with ID: {Id}", todoItem.Id);

            var existingTodo = await _context.TodoItems.FindAsync(todoItem.Id);

            if (existingTodo == null)
            {
                _logger.LogWarning("Todo item with ID {Id} not found for update", todoItem.Id);
                throw new InvalidOperationException($"Todo with ID {todoItem.Id} not found");
            }

            // Track completion status change
            var wasCompleted = existingTodo.IsCompleted;
            var isNowCompleted = todoItem.IsCompleted;

            // Update properties with sanitized input
            existingTodo.Title = todoItem.Title.Trim();
            existingTodo.Description = string.IsNullOrWhiteSpace(todoItem.Description)
                ? string.Empty
                : todoItem.Description.Trim();
            existingTodo.IsCompleted = isNowCompleted;

            // Handle completion timestamp logic
            existingTodo.CompletedAt = (isNowCompleted, wasCompleted) switch
            {
                (true, false) => DateTime.UtcNow, // Just completed
                (false, true) => null, // Uncompleted  
                (false, false) => null, // Still not completed
                (true, true) => existingTodo.CompletedAt // Still completed, keep original timestamp
            };

            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully updated todo item with ID: {Id}. Completion changed: {WasCompleted} -> {IsCompleted}",
                todoItem.Id, wasCompleted, isNowCompleted);

            return existingTodo;
        }
        catch (InvalidOperationException)
        {
            // Re-throw without wrapping
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating todo item with ID: {Id}", todoItem.Id);
            throw new InvalidOperationException($"Failed to update todo item with ID {todoItem.Id}", ex);
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid todo item ID provided for deletion: {Id}", id);
            return false;
        }

        try
        {
            _logger.LogDebug("Deleting todo item with ID: {Id}", id);

            var todo = await _context.TodoItems.FindAsync(id);

            if (todo == null)
            {
                _logger.LogWarning("Todo item with ID {Id} not found for deletion", id);
                return false;
            }

            _context.TodoItems.Remove(todo);
            var changesCount = await _context.SaveChangesAsync();

            var deleted = changesCount > 0;
            if (deleted)
            {
                _logger.LogInformation("Successfully deleted todo item with ID: {Id}", id);
            }
            else
            {
                _logger.LogWarning("No changes were made when attempting to delete todo item with ID: {Id}", id);
            }

            return deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting todo item with ID: {Id}", id);
            throw new InvalidOperationException($"Failed to delete todo item with ID {id}", ex);
        }
    }

    public async Task<int> GetTotalCountAsync()
    {
        try
        {
            var count = await _context.TodoItems.CountAsync();
            _logger.LogDebug("Total todo items count: {Count}", count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total todo items count");
            throw new InvalidOperationException("Failed to get total todo items count", ex);
        }
    }

    public async Task<int> GetCompletedCountAsync()
    {
        try
        {
            var count = await _context.TodoItems.CountAsync(t => t.IsCompleted);
            _logger.LogDebug("Completed todo items count: {Count}", count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting completed todo items count");
            throw new InvalidOperationException("Failed to get completed todo items count", ex);
        }
    }

    public async Task<int> GetPendingCountAsync()
    {
        try
        {
            var count = await _context.TodoItems.CountAsync(t => !t.IsCompleted);
            _logger.LogDebug("Pending todo items count: {Count}", count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending todo items count");
            throw new InvalidOperationException("Failed to get pending todo items count", ex);
        }
    }

    public async Task<IEnumerable<TodoItem>> GetRecentAsync(int count = 10)
    {
        if (count <= 0)
        {
            throw new ArgumentException("Count must be greater than zero", nameof(count));
        }

        try
        {
            _logger.LogDebug("Retrieving {Count} recent todo items", count);

            var todos = await _context.TodoItems
                .OrderByDescending(t => t.CreatedAt)
                .Take(count)
                .AsNoTracking()
                .ToListAsync();

            _logger.LogDebug("Retrieved {ActualCount} recent todo items", todos.Count);
            return todos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recent todo items");
            throw new InvalidOperationException("Failed to retrieve recent todo items", ex);
        }
    }

    public async Task<bool> BulkUpdateCompletionStatusAsync(IEnumerable<int> todoIds, bool isCompleted)
    {
        ArgumentNullException.ThrowIfNull(todoIds);

        var ids = todoIds.Where(id => id > 0).ToList();
        if (!ids.Any())
        {
            _logger.LogWarning("No valid todo IDs provided for bulk update");
            return false;
        }

        try
        {
            _logger.LogDebug("Bulk updating completion status for {Count} todos to {Status}",
                ids.Count, isCompleted);

            var todos = await _context.TodoItems
                .Where(t => ids.Contains(t.Id))
                .ToListAsync();

            if (!todos.Any())
            {
                _logger.LogWarning("No todos found for bulk update with provided IDs");
                return false;
            }

            var updatedCount = 0;
            var timestamp = DateTime.UtcNow;

            foreach (var todo in todos)
            {
                if (todo.IsCompleted != isCompleted)
                {
                    todo.IsCompleted = isCompleted;
                    todo.CompletedAt = isCompleted ? timestamp : null;
                    updatedCount++;
                }
            }

            if (updatedCount > 0)
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Bulk updated {UpdatedCount} todos to completion status: {Status}",
                    updatedCount, isCompleted);
            }
            else
            {
                _logger.LogInformation("No todos needed updating for bulk completion status change");
            }

            return updatedCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during bulk update of completion status");
            throw new InvalidOperationException("Failed to bulk update completion status", ex);
        }
    }
}
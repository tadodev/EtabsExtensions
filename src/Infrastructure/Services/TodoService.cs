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
        _context = context;
        _logger = logger;
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
            throw;
        }
    }

    public async Task<TodoItem?> GetByIdAsync(int id)
    {
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

            return todo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving todo item with ID: {Id}", id);
            throw;
        }
    }

    public async Task<TodoItem> CreateAsync(TodoItem todoItem)
    {
        try
        {
            _logger.LogDebug("Creating new todo item: {Title}", todoItem.Title);

            // Ensure proper timestamps
            todoItem.CreatedAt = DateTime.UtcNow;
            todoItem.CompletedAt = null; // New todos are not completed

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created todo item with ID: {Id}", todoItem.Id);
            return todoItem;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating todo item: {Title}", todoItem.Title);
            throw;
        }
    }

    public async Task<TodoItem> UpdateAsync(TodoItem todoItem)
    {
        try
        {
            _logger.LogDebug("Updating todo item with ID: {Id}", todoItem.Id);

            var existingTodo = await _context.TodoItems.FindAsync(todoItem.Id);

            if (existingTodo == null)
            {
                _logger.LogWarning("Todo item with ID {Id} not found for update", todoItem.Id);
                throw new InvalidOperationException($"Todo with ID {todoItem.Id} not found");
            }

            // Update properties
            existingTodo.Title = todoItem.Title;
            existingTodo.Description =
                string.IsNullOrWhiteSpace(todoItem.Description) ? string.Empty : todoItem.Description;

            // Handle completion status change
            var wasCompleted = existingTodo.IsCompleted;
            existingTodo.IsCompleted = todoItem.IsCompleted;

            existingTodo.CompletedAt = todoItem.IsCompleted switch
            {
                true when !wasCompleted => DateTime.UtcNow, // Just completed
                false => null, // Uncompleted
                _ => existingTodo.CompletedAt // No change in completion status
            };

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated todo item with ID: {Id}", todoItem.Id);
            return existingTodo;
        }
        catch (Exception ex) when (!(ex is InvalidOperationException))
        {
            _logger.LogError(ex, "Error updating todo item with ID: {Id}", todoItem.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
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
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted todo item with ID: {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting todo item with ID: {Id}", id);
            throw;
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
            throw;
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
            throw;
        }
    }
}
using EtabsExtensions.Core.Models;
using EtabsExtensions.Core.Services;
using EtabsExtensions.Infrastructure.Data;

namespace EtabsExtensions.Infrastructure.Services;

public class TodoService: ITodoService
{
    private readonly TodoDbContext _context;

    public TodoService(TodoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TodoItem>> GetAllAsync()
    {
        return await _context.TodoItems
            .OrderByDescending(t => t.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<TodoItem?> GetByIdAsync(int id)
    {
        return await _context.TodoItems
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<TodoItem> CreateAsync(TodoItem todoItem)
    {
        todoItem.CreatedAt = DateTime.UtcNow;

        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync();

        return todoItem;
    }

    public async Task<TodoItem> UpdateAsync(TodoItem todoItem)
    {
        var existingTodo = await _context.TodoItems.FindAsync(todoItem.Id);

        if (existingTodo == null)
        {
            throw new InvalidOperationException($"Todo with ID {todoItem.Id} not found");
        }

        //update properties
        existingTodo.Title = todoItem.Title;
        existingTodo.Description = string.IsNullOrWhiteSpace(todoItem.Description) ? string.Empty : todoItem.Description;
        existingTodo.IsCompleted = todoItem.IsCompleted;

        existingTodo.CompletedAt = todoItem.IsCompleted switch
        {
            true when existingTodo.CompletedAt == null => DateTime.UtcNow,
            false => null,
            _ => existingTodo.CompletedAt
        };

        await _context.SaveChangesAsync();

        return existingTodo;


    }

    public async Task<bool> DeleteAsync(int id)
    {
        var todo = await _context.TodoItems.FindAsync(id);

        if (todo == null)
        {
            throw new InvalidOperationException($"Todo with ID {id} not found");
        }

        _context.TodoItems.Remove(todo);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.TodoItems.CountAsync();
    }

    public Task<int> GetCompletedCountAsync()
    {
        return _context.TodoItems.CountAsync(t => t.IsCompleted);
    }
}
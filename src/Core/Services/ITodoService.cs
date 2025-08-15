using EtabsExtensions.Core.Models;

namespace EtabsExtensions.Core.Services;

public interface ITodoService
{
    Task<IEnumerable<TodoItem>> GetAllAsync();

    Task<TodoItem?> GetByIdAsync(int id);

    Task<TodoItem> CreateAsync(TodoItem todoItem);

    Task<TodoItem> UpdateAsync(TodoItem todoItem);

    Task<bool> DeleteAsync(int id);

    Task<int> GetTotalCountAsync();

    Task<int> GetCompletedCountAsync();
}

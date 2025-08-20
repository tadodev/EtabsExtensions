using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EtabsExtensions.Core.Models;
using EtabsExtensions.Core.Services;
using System.Collections.ObjectModel;

namespace EtabsExtensions.Core.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ITodoService _todoService;

    public MainViewModel(ITodoService todoService)
    {
        _todoService = todoService;
        TodoItems = new ObservableCollection<TodoItemViewModel>();
    }

    // Observable properties for UI binding
    [ObservableProperty]
    private ObservableCollection<TodoItemViewModel> _todoItems;

    [ObservableProperty]
    private string _newTodoTitle = string.Empty;

    [ObservableProperty]
    private string _newTodoDescription = string.Empty;

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private int _completedCount;

    [ObservableProperty]
    private int _pendingCount; // Changed from computed property to observable property

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    // Command to Load Todos from the service
    [RelayCommand]
    private async Task LoadTodosAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading todos...";

            var todos = await _todoService.GetAllAsync();

            TodoItems.Clear();
            foreach (var todo in todos.OrderByDescending(t => t.CreatedAt))
            {
                TodoItems.Add(new TodoItemViewModel(todo, _todoService));
            }

            await UpdateCountsAsync();

            StatusMessage = $"Loaded {TodoItems.Count} todos.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading todos: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanAddTodos))]
    private async Task AddTodoAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Adding new todo...";

            var newTodo = new TodoItem
            {
                Title = NewTodoTitle.Trim(),
                Description = string.IsNullOrWhiteSpace(NewTodoDescription) ? string.Empty : NewTodoDescription.Trim(),
            };

            var createTodo = await _todoService.CreateAsync(newTodo);

            var newTodoViewModel = new TodoItemViewModel(createTodo, _todoService);

            TodoItems.Insert(0, newTodoViewModel); // Insert at top for better UX

            NewTodoTitle = string.Empty;
            NewTodoDescription = string.Empty;

            await UpdateCountsAsync();

            StatusMessage = "New todo added successfully.";

            AddTodoCommand.NotifyCanExecuteChanged();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error adding todo: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeleteTodoAsync(TodoItemViewModel todoItemViewModel)
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Deleting todo...";

            if (await _todoService.DeleteAsync(todoItemViewModel.Id))
            {
                TodoItems.Remove(todoItemViewModel);
                await UpdateCountsAsync();
                StatusMessage = "Todo deleted successfully.";
            }
            else
            {
                StatusMessage = "Failed to delete todo.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error deleting todo: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SaveTodoAsync(TodoItemViewModel todoViewModel)
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Saving todo...";

            await _todoService.UpdateAsync(todoViewModel.ToModel());

            await UpdateCountsAsync();
            todoViewModel.IsEditing = false;
            StatusMessage = "Todo saved successfully.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error saving todo: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ToggleTodoCompletedAsync(TodoItemViewModel todoItemViewModel)
    {
        try
        {
            todoItemViewModel.ToggleComplete();
            await _todoService.UpdateAsync(todoItemViewModel.ToModel());
            await UpdateCountsAsync();
            StatusMessage = todoItemViewModel.IsCompleted ? "Todo marked as completed!" : "Todo marked as pending.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error updating todo: {ex.Message}";
            // Revert the change if update failed
            todoItemViewModel.ToggleComplete();
        }
    }

    private bool CanAddTodos()
    {
        return !string.IsNullOrWhiteSpace(NewTodoTitle) && !IsLoading;
    }

    // Update counts for total and completed todos
    private async Task UpdateCountsAsync()
    {
        TotalCount = await _todoService.GetTotalCountAsync();
        CompletedCount = await _todoService.GetCompletedCountAsync();
        PendingCount = TotalCount - CompletedCount; // Update pending count explicitly
    }

    partial void OnNewTodoTitleChanged(string value)
    {
        AddTodoCommand.NotifyCanExecuteChanged();
    }

    public async Task InitializeAsync()
    {
        await LoadTodosAsync();
    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EtabsExtensions.Core.Services;
using System.Collections.ObjectModel;

namespace EtabsExtensions.Core.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public readonly ITodoService _todoService;

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
    private string _statusMessage = string.Empty;

    // Computed property for pending count
    public int PendingCount => TotalCount - CompletedCount;


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

            }
        }
        catch (Exception)
        {

            throw;
        }
    }

}

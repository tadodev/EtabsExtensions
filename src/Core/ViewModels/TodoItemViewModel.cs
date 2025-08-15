using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EtabsExtensions.Core.Models;

namespace EtabsExtensions.Core.ViewModels;

public partial class TodoItemViewModel : ObservableObject
{
    private readonly TodoItem _todoItem;

    public TodoItemViewModel(TodoItem todoItem)
    {
        _todoItem = todoItem;
        Title = _todoItem.Title;
        Description = _todoItem.Description ?? string.Empty;
        IsCompleted = _todoItem.IsCompleted;
    }

    // Properties to expose TodoItem properties
    public int Id => _todoItem.Id;
    public DateTime CreatedAt => _todoItem.CreatedAt;
    public DateTime? CompletedAt => _todoItem.CompletedAt;

    // Observable properties for UI binding
    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private bool _isCompleted;

    [ObservableProperty]
    private bool _isEditing;

    [RelayCommand]
    private void ToggleEditMode()
    {
        IsEditing = !IsEditing;
    }

    [RelayCommand]
    private void ToggleComplete()
    {
        IsCompleted = !IsCompleted;
        OnPropertyChanged(nameof(IsCompleted));
    }

    public TodoItem ToModel()
    {
        _todoItem.Title = Title;
        _todoItem.Description = string.IsNullOrWhiteSpace(Description) ? string.Empty : Description;
        _todoItem.IsCompleted = IsCompleted;

        if (IsCompleted && _todoItem.CompletedAt == null)
        {
            _todoItem.CompletedAt = DateTime.UtcNow;
        }
        else if (!IsCompleted)
        {
            _todoItem.CompletedAt = null;
        }

        return _todoItem;
    }
}

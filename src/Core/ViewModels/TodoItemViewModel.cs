using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EtabsExtensions.Core.Models;
using EtabsExtensions.Core.Services;

namespace EtabsExtensions.Core.ViewModels;

public partial class TodoItemViewModel : ObservableObject
{
    private readonly TodoItem _todoItem;
    private readonly ITodoService _todoService;

    public TodoItemViewModel(TodoItem todoItem, ITodoService todoService)
    {
        _todoItem = todoItem;
        _todoService = todoService;
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
    public void ToggleComplete()
    {
        IsCompleted = !IsCompleted;
        OnPropertyChanged(nameof(IsCompleted));
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        try
        {
            await _todoService.UpdateAsync(ToModel());
            IsEditing = false;
        }
        catch (Exception ex)
        {
            // Handle error - could notify parent or show message
            throw new InvalidOperationException($"Failed to save todo: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Returns the underlying TodoItem model with updated properties to Database.
    /// </summary>
    /// <returns></returns>
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

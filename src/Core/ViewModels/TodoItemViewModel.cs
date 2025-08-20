using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EtabsExtensions.Core.Models;
using EtabsExtensions.Core.Services;
using Microsoft.Extensions.Logging;

namespace EtabsExtensions.Core.ViewModels;

public partial class TodoItemViewModel : ObservableObject
{
    private readonly TodoItem _todoItem;
    private readonly ITodoService _todoService;
    private readonly ILogger<TodoItemViewModel>? _logger;

    public TodoItemViewModel(TodoItem todoItem, ITodoService todoService, ILogger<TodoItemViewModel>? logger = null)
    {
        _todoItem = todoItem;
        _todoService = todoService;
        _logger = logger;

        Title = _todoItem.Title;
        Description = _todoItem.Description ?? string.Empty;
        IsCompleted = _todoItem.IsCompleted;

        _logger?.LogDebug("Created TodoItemViewModel for item with ID: {TodoId}", todoItem.Id);
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

    [ObservableProperty]
    private bool _isSaving;

    // Computed properties
    public string DisplayCreatedAt => CreatedAt.ToString("MMM dd, yyyy HH:mm");
    public string? DisplayCompletedAt => CompletedAt?.ToString("MMM dd, yyyy HH:mm");
    public bool HasDescription => !string.IsNullOrWhiteSpace(Description);

    [RelayCommand]
    private void ToggleEditMode()
    {
        if (IsSaving) return;

        IsEditing = !IsEditing;
        _logger?.LogDebug("Toggled edit mode for TodoItem {TodoId}. IsEditing: {IsEditing}", Id, IsEditing);
    }

    [RelayCommand]
    public void ToggleComplete()
    {
        if (IsSaving) return;

        var wasCompleted = IsCompleted;
        IsCompleted = !IsCompleted;

        _logger?.LogDebug("Toggled completion status for TodoItem {TodoId}. Was: {WasCompleted}, Now: {IsCompleted}",
            Id, wasCompleted, IsCompleted);

        OnPropertyChanged(nameof(IsCompleted));
        OnPropertyChanged(nameof(DisplayCompletedAt));
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsSaving) return;

        try
        {
            IsSaving = true;
            _logger?.LogDebug("Saving TodoItem {TodoId}", Id);

            await _todoService.UpdateAsync(ToModel());
            IsEditing = false;

            _logger?.LogInformation("Successfully saved TodoItem {TodoId}", Id);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to save TodoItem {TodoId}", Id);
            throw new InvalidOperationException($"Failed to save todo: {ex.Message}", ex);
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private void CancelEdit()
    {
        if (IsSaving) return;

        // Revert changes
        Title = _todoItem.Title;
        Description = _todoItem.Description ?? string.Empty;
        IsCompleted = _todoItem.IsCompleted;
        IsEditing = false;

        _logger?.LogDebug("Cancelled edit for TodoItem {TodoId}", Id);
    }

    /// <summary>
    /// Returns the underlying TodoItem model with updated properties to Database.
    /// </summary>
    /// <returns></returns>
    public TodoItem ToModel()
    {
        _todoItem.Title = Title?.Trim() ?? string.Empty;
        _todoItem.Description = string.IsNullOrWhiteSpace(Description) ? string.Empty : Description.Trim();
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

    // Property change notifications for computed properties
    partial void OnIsCompletedChanged(bool value)
    {
        OnPropertyChanged(nameof(DisplayCompletedAt));
    }

    partial void OnTitleChanged(string value)
    {
        // Auto-save could be implemented here if needed
    }

    partial void OnDescriptionChanged(string value)
    {
        OnPropertyChanged(nameof(HasDescription));
    }
}
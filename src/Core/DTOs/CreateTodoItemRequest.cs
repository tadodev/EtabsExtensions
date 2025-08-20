namespace EtabsExtensions.Core.DTOs;

/// <summary>
/// Data Transfer Object for creating a new TodoItem
/// </summary>
public record CreateTodoItemRequest
{
    public required string Title { get; init; }
    public string? Description { get; init; }
}

/// <summary>
/// Data Transfer Object for updating an existing TodoItem
/// </summary>
public record UpdateTodoItemRequest
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public bool IsCompleted { get; init; }
}

/// <summary>
/// Data Transfer Object for TodoItem response
/// </summary>
public record TodoItemResponse
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public bool IsCompleted { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? CompletedAt { get; init; }

    // Computed properties for UI
    public string DisplayCreatedAt => CreatedAt.ToString("MMM dd, yyyy HH:mm");
    public string? DisplayCompletedAt => CompletedAt?.ToString("MMM dd, yyyy HH:mm");
    public bool HasDescription => !string.IsNullOrWhiteSpace(Description);
}

/// <summary>
/// Data Transfer Object for todo statistics
/// </summary>
public record TodoStatsResponse
{
    public int TotalCount { get; init; }
    public int CompletedCount { get; init; }
    public int PendingCount { get; init; }
    public double CompletionPercentage => TotalCount > 0 ? (double)CompletedCount / TotalCount * 100 : 0;
}

/// <summary>
/// Data Transfer Object for bulk operations
/// </summary>
public record BulkUpdateRequest
{
    public required IEnumerable<int> TodoIds { get; init; }
    public bool IsCompleted { get; init; }
}

/// <summary>
/// Data Transfer Object for search/filter operations
/// </summary>
public record TodoSearchRequest
{
    public string? SearchTerm { get; init; }
    public bool? IsCompleted { get; init; }
    public DateTime? CreatedAfter { get; init; }
    public DateTime? CreatedBefore { get; init; }
    public int Skip { get; init; } = 0;
    public int Take { get; init; } = 50;
    public string SortBy { get; init; } = "CreatedAt";
    public bool SortDescending { get; init; } = true;
}
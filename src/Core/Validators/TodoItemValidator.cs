using EtabsExtensions.Core.DTOs;

namespace EtabsExtensions.Core.Validators;

/// <summary>
/// Validator for CreateTodoItemRequest
/// </summary>
public class CreateTodoItemRequestValidator : AbstractValidator<CreateTodoItemRequest>
{
    public CreateTodoItemRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters")
            .Must(BeValidTitle)
            .WithMessage("Title cannot contain only whitespace");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters");
    }

    private static bool BeValidTitle(string? title)
    {
        return !string.IsNullOrWhiteSpace(title);
    }
}

/// <summary>
/// Validator for UpdateTodoItemRequest
/// </summary>
public class UpdateTodoItemRequestValidator : AbstractValidator<UpdateTodoItemRequest>
{
    public UpdateTodoItemRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Valid ID is required");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters")
            .Must(BeValidTitle)
            .WithMessage("Title cannot contain only whitespace");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters");
    }

    private static bool BeValidTitle(string? title)
    {
        return !string.IsNullOrWhiteSpace(title);
    }
}

/// <summary>
/// Validator for BulkUpdateRequest
/// </summary>
public class BulkUpdateRequestValidator : AbstractValidator<BulkUpdateRequest>
{
    public BulkUpdateRequestValidator()
    {
        RuleFor(x => x.TodoIds)
            .NotEmpty()
            .WithMessage("At least one Todo ID is required")
            .Must(ContainOnlyPositiveIds)
            .WithMessage("All Todo IDs must be positive integers");
    }

    private static bool ContainOnlyPositiveIds(IEnumerable<int> ids)
    {
        return ids.All(id => id > 0);
    }
}

/// <summary>
/// Validator for TodoSearchRequest
/// </summary>
public class TodoSearchRequestValidator : AbstractValidator<TodoSearchRequest>
{
    public TodoSearchRequestValidator()
    {
        RuleFor(x => x.Skip)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Skip must be non-negative");

        RuleFor(x => x.Take)
            .GreaterThan(0)
            .LessThanOrEqualTo(1000)
            .WithMessage("Take must be between 1 and 1000");

        RuleFor(x => x.SortBy)
            .Must(BeValidSortField)
            .WithMessage("SortBy must be one of: CreatedAt, Title, IsCompleted, CompletedAt");

        RuleFor(x => x.CreatedAfter)
            .LessThan(x => x.CreatedBefore)
            .When(x => x.CreatedAfter.HasValue && x.CreatedBefore.HasValue)
            .WithMessage("CreatedAfter must be before CreatedBefore");
    }

    private static bool BeValidSortField(string sortBy)
    {
        var validFields = new[] { "CreatedAt", "Title", "IsCompleted", "CompletedAt" };
        return validFields.Contains(sortBy);
    }
}
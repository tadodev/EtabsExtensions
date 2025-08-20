using EtabsExtensions.Core.Models;

namespace EtabsExtensions.Core.Validators;

/// <summary>
/// Validator for TodoItem using FluentValidation
/// </summary>
public class TodoItemValidator : AbstractValidator<TodoItem>
{
    public TodoItemValidator()
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

        RuleFor(x => x.CreatedAt)
            .NotEmpty()
            .WithMessage("Created date is required")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Created date cannot be in the future");

        RuleFor(x => x.CompletedAt)
            .Must((todoItem, completedAt) => ValidateCompletedAt(todoItem, completedAt))
            .WithMessage("Completed date must be set only when todo is completed and cannot be before creation date");
    }

    private static bool BeValidTitle(string? title)
    {
        return !string.IsNullOrWhiteSpace(title);
    }

    private static bool ValidateCompletedAt(TodoItem todoItem, DateTime? completedAt)
    {
        // If not completed, CompletedAt should be null
        if (!todoItem.IsCompleted)
        {
            return completedAt == null;
        }

        // If completed, CompletedAt should be set and after CreatedAt
        if (todoItem.IsCompleted)
        {
            return completedAt.HasValue && completedAt.Value >= todoItem.CreatedAt;
        }

        return true;
    }
}
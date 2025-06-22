using FluentValidation.Results;

namespace EtabsExtensions.Core.Common.Exceptions;

public class ValidationException : Exception
{
    public ValidationException()
        : base("One or more validation failure have occurred")
    {
        Errors = new Dictionary<string, string[]>();
    }
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public IDictionary<string, string[]> Errors { get; }
}

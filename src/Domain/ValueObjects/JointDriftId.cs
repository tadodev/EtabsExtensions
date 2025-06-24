using EtabsExtensions.Domain.Common;

namespace EtabsExtensions.Domain.ValueObjects;

public sealed class JointDriftId : ValueObject
{
    /// <summary>
    /// Create a unique identifier for a joint drift item.
    /// </summary>
    public string? Story { get; }
    public string? Label { get; }
    public string? OutputCase { get; }
    public double StepNumber { get; }

    public JointDriftId(string? story, string? label, string? outputCase, double stepNumber)
    {
        Story = story;
        Label = label;
        OutputCase = outputCase;
        StepNumber = stepNumber;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Story!;
        yield return Label!;
        yield return OutputCase!;
        yield return StepNumber;
    }
}

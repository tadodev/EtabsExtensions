using EtabsExtensions.Domain.Common;

namespace EtabsExtensions.Domain.ValueObjects;

public sealed class JointDriftId : ValueObject
{
    /// <summary>
    /// Create a unique identifier for a joint drift item.
    /// </summary>
    public string? Story { get; }
    public int Label { get; }
    public string? OutputCase { get; }
    public int StepNumber { get; }

    public JointDriftId(string? story, int label, string? outputCase, int stepNumber)
    {
        Story = story;
        Label = label;
        OutputCase = outputCase;
        StepNumber = stepNumber;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Story!;
        yield return Label;
        yield return OutputCase!;
        yield return StepNumber;
    }
}

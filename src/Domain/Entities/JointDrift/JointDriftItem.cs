namespace EtabsExtensions.Domain.Entities.JointDrift;

public class JointDriftItem
{
    public string? Story { get; set; }
    public string? Label { get; set; }
    public string? UniqueName { get; set; }
    public string? OutputCase { get; set; }
    public string? CaseType { get; set; }
    public string? StepType { get; set; }
    public double StepNumber { get; set; }
    public string? StepLabel { get; set; }
    public double DispX { get; set; }
    public double DispY { get; set; }
    public double DriftX { get; set; }
    public double DriftY { get; set; }

    public JointDriftItem(string? story, string? label, string? uniqueName, string? outputCase, string? caseType, string? stepType, double stepNumber, string? stepLabel, double dispX, double dispY, double driftX, double driftY)
    {
        Story = story;
        Label = label;
        UniqueName = uniqueName;
        OutputCase = outputCase;
        CaseType = caseType;
        StepType = stepType;
        StepNumber = stepNumber;
        StepLabel = stepLabel;
        DispX = dispX;
        DispY = dispY;
        DriftX = driftX;
        DriftY = driftY;
    }
}

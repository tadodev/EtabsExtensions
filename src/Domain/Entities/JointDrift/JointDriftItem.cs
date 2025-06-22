namespace EtabsExtensions.Domain.Entities.JointDrift;

public class JointDriftItem
{
    public string? Story { get; set; }
    public int Label { get; set; }
    public int UniqueName { get; set; }
    public string? OutputCase { get; set; }
    public int StepNumber { get; set; }
    public double DispX { get; set; }
    public double DispY { get; set; }
    public double DriftX { get; set; }
    public double Drifty { get; set; }

    public JointDriftItem(string? story, int label, int uniqueName, string? outputCase,
        int stepNumber, double dispX, double dispY, double driftX, double drifty)
    {
        Story = story;
        Label = label;
        UniqueName = uniqueName;
        OutputCase = outputCase;
        StepNumber = stepNumber;
        DispX = dispX;
        DispY = dispY;
        DriftX = driftX;
        Drifty = drifty;
    }
}

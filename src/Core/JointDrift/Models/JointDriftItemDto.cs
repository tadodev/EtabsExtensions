namespace EtabsExtensions.Core.JointDrift.Models;

public class JointDriftItemDto
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
}

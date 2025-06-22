namespace EtabsExtensions.Domain.Exceptions;

public class JointDriftException : Exception
{
    public JointDriftException(string message)
        : base(message)
    {
    }

    public static JointDriftException ForLabelNotFound(int label)
    {
        return new JointDriftException($"Unable to find joint drift item with label: {label}");
    }

    public static JointDriftException ForOutputCaseNotFound(string outputCase)
    {
        return new JointDriftException($"Unable to find joint drift item with output case: {outputCase}");
    }
}

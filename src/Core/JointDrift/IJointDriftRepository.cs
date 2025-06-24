using EtabsExtensions.Domain.Entities.JointDrift;

namespace EtabsExtensions.Core.JointDrift;

public interface IJointDriftRepository
{
    /// <summary>
    /// Asynchronously retrieves a list of unique "Output Case" names from the data source.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of unique case name strings.</returns>
    Task<IEnumerable<string>> GetUniqueCaseNamesAsync();

    /// <summary>
    /// Asynchronously retrieves all joint drift entries that match a specific output case.
    /// </summary>
    /// <param name="outputCase">The selected output case to filter the data by.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of JointDriftEntry objects.</returns>
    Task<IEnumerable<JointDriftItem>> GetEntriesByCaseAsync(string outputCase);
}

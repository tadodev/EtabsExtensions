using EtabsExtensions.Domain.Entities.JointDrift;

namespace EtabsExtensions.Core.JointDrift
{
    public interface IJointDriftRepository
    {
        Task AddItemAsync(JointDriftItem item, CancellationToken cancellationToken = default);
        Task AddItemsAsync(IEnumerable<JointDriftItem> items, CancellationToken cancellationToken = default);
        Task<IList<JointDriftItem>> GetAllItemsAsync(CancellationToken cancellationToken = default);
        // Add more CRUD methods as needed
    }
}

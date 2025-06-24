using EtabsExtensions.Domain.Entities.JointDrift;
using EtabsExtensions.Core.JointDrift;

namespace EtabsExtensions.Core.JointDrift;

public class JointDriftService
{
    private readonly IJointDriftRepository _jointDriftRepository;

    public JointDriftService(IJointDriftRepository jointDriftRepository)
    {
        _jointDriftRepository = jointDriftRepository;
    }

    public async Task<IEnumerable<string>> GetUniqueCaseNamesAsync()
    {
        return await _jointDriftRepository.GetUniqueCaseNamesAsync();
    }

    public async Task<IEnumerable<JointDriftItem>> GetEntriesByCaseAsync(string outputCase)
    {
        return await _jointDriftRepository.GetEntriesByCaseAsync(outputCase);
    }
}
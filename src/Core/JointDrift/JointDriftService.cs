using EtabsExtensions.Core.JointDrift.Models;
using EtabsExtensions.Domain.Entities.JointDrift;

namespace EtabsExtensions.Core.JointDrift;

public class JointDriftService
{
    private readonly IJointDriftRepository _repository;
    private readonly IMapper _mapper;

    public JointDriftService(IJointDriftRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IList<JointDriftItemDto>> GetAllItemsAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetAllItemsAsync(cancellationToken);
        return _mapper.Map<IList<JointDriftItemDto>>(items);
    }

    public async Task AddItemsAsync(IEnumerable<JointDriftItem> jointDriftItems, CancellationToken cancellationToken = default)
    {
        var mappedItems = _mapper.Map<IEnumerable<JointDriftItem>>(jointDriftItems);
        await _repository.AddItemsAsync(mappedItems, cancellationToken);
    }
    // Add more service methods as needed
}

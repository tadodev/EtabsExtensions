using EtabsExtensions.Domain.ValueObjects;

namespace EtabsExtensions.Domain.Entities.JointDrift;

public class JointDriftList
{
    public IList<JointDriftItem> Items { get; set; } = new List<JointDriftItem>();

    public IEnumerable<JointDriftItem> GetItemsByIds(IEnumerable<JointDriftId> ids)
    {
        var idSet = new HashSet<JointDriftId>(ids);
        return Items.Where(item => idSet.Contains(new JointDriftId(item.Story, item.Label, item.OutputCase, item.StepNumber)));
    }
}

using EtabsExtensions.Domain.Entities.JointDrift;

namespace EtabsExtensions.Domain.Events;

public class JointDriftItemCreatedEvent : BaseEvent
{
    public JointDriftItemCreatedEvent(JointDriftItem jointDriftItem)
    {
        Item = jointDriftItem ?? throw new ArgumentNullException(nameof(jointDriftItem), "JointDriftItem cannot be null.");
    }
    public JointDriftItem Item { get; }
}

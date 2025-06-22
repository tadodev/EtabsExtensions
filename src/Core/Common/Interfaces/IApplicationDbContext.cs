using EtabsExtensions.Domain.Entities.JointDrift;
using Microsoft.EntityFrameworkCore;

namespace EtabsExtensions.Core.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<JointDriftList>? JointDriftLists { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

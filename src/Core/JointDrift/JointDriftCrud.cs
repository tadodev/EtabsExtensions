using EtabsExtensions.Core.Common.Interfaces;
using EtabsExtensions.Domain.Entities.JointDrift;
using FluentValidation.Results;
using CoreValidationException = EtabsExtensions.Core.Common.Exceptions.ValidationException;

namespace EtabsExtensions.Core.JointDrift;

public class JointDriftRepository : IJointDriftRepository
{
    private readonly IApplicationDbContext _context;
    private readonly IValidator<JointDriftItem> _validator;

    public JointDriftRepository(IApplicationDbContext context, IValidator<JointDriftItem> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task AddItemAsync(JointDriftItem item, CancellationToken cancellationToken = default)
    {
        ValidationResult result = await _validator.ValidateAsync(item, cancellationToken);
        if (!result.IsValid)
            throw new CoreValidationException(result.Errors);

        if (_context.JointDriftLists == null)
            throw new Exception("JointDriftLists DbSet is not initialized.");

        var list = _context.JointDriftLists!.FirstOrDefault();
        if (list == null)
            throw new Exception("No JointDriftList found.");
        list.Items.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddItemsAsync(IEnumerable<JointDriftItem> items, CancellationToken cancellationToken = default)
    {
        if (_context.JointDriftLists == null)
            throw new Exception("JointDriftLists DbSet is not initialized.");
        var list = _context.JointDriftLists!.FirstOrDefault();
        if (list == null)
            throw new Exception("No JointDriftList found.");

        var allFailures = new List<ValidationFailure>();
        foreach (var item in items)
        {
            var result = await _validator.ValidateAsync(item, cancellationToken);
            if (!result.IsValid)
                allFailures.AddRange(result.Errors);
            else
                list.Items.Add(item);
        }
        if (allFailures.Count > 0)
            throw new CoreValidationException(allFailures);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<IList<JointDriftItem>> GetAllItemsAsync(CancellationToken cancellationToken = default)
    {
        if (_context.JointDriftLists == null)
            throw new Exception("JointDriftLists DbSet is not initialized.");
        var list = _context.JointDriftLists!.FirstOrDefault();
        return Task.FromResult(list?.Items ?? new List<JointDriftItem>());
    }
}

#pragma warning disable CA1416
using System.Threading.Tasks;
using EtabsExtensions.Infrastructure.Import.Interface;
using EtabsExtensions.Infrastructure.Import.Repositories.JointDrift;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;
using EtabsExtensions.Infrastructure.Import;

namespace Infrastructure.IntegrationTests;

[TestFixture]
[Platform(Include = "Win")] // Only run on Windows
public class AccessJointDriftRepositoryTests
{
    private const string AccessFilePath = @"D:\Research\EtabsExtensions\BallyIR_postprocessing_data.accdb";

    [Test]
    public async Task Can_Connect_And_Query_JointDrifts()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IAccessConnectionProvider, AccessConnectionProvider>();
        services.AddScoped<AccessJointDriftRepository>();
        var provider = services.BuildServiceProvider();

        var connectionProvider = provider.GetRequiredService<IAccessConnectionProvider>();
        var connected = await connectionProvider.OpenAsync(AccessFilePath);
        Assert.That(connected, Is.True);

        var repo = provider.GetRequiredService<AccessJointDriftRepository>();
        var caseNames = await repo.GetUniqueCaseNamesAsync();
        Assert.That(caseNames, Is.Not.Null.And.Not.Empty);

        var firstCase = "ELF X";
        var entries = await repo.GetEntriesByCaseAsync(firstCase);
        Assert.That(entries, Is.Not.Null.And.Not.Empty);

        await connectionProvider.CloseAsync();
    }
}
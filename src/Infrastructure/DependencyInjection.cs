using Microsoft.Extensions.Hosting;
using System.Runtime.Versioning;

namespace Microsoft.Extensions.DependencyInjection;
public static class DependencyInjection
{
    [SupportedOSPlatform("windows")]
    public static void AddInfrastructureServices(this IHostApplicationBuilder services)
    {
        // Register Access connection provider as singleton
        // Register import repository for joint drift
    }
}


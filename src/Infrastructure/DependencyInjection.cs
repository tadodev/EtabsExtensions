using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Versioning;
using EtabsExtensions.Core.Common.Interfaces;
using EtabsExtensions.Core.JointDrift;
using Microsoft.Extensions.Hosting;
using EtabsExtensions.Infrastructure.Import.Interface;
using EtabsExtensions.Infrastructure.Import;
using EtabsExtensions.Infrastructure.Import.Repositories.JointDrift;

namespace Microsoft.Extensions.DependencyInjection;
public static class DependencyInjection
{
    [SupportedOSPlatform("windows")]
    public static void AddInfrastructureServices(this IHostApplicationBuilder services)
    {
        // Register Access connection provider as singleton
        services.Services.AddSingleton<IAccessConnectionProvider, AccessConnectionProvider>();
        // Register import repository for joint drift
        services.Services.AddScoped<IJointDriftRepository, AccessJointDriftRepository>();
    }
}


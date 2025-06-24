using EtabsExtensions.Domain.Entities.JointDrift;
using EtabsExtensions.Core.JointDrift;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddCoreServices(this IHostApplicationBuilder builder)
    {
        // Register service
        builder.Services.AddScoped<JointDriftService>();
        // Register AutoMapper (scan current assembly)
        builder.Services.AddAutoMapper(typeof(DependencyInjection).Assembly);
    }
}

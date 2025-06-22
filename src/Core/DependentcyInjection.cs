using EtabsExtensions.Core.JointDrift;
using EtabsExtensions.Core.JointDrift.Validators;
using EtabsExtensions.Domain.Entities.JointDrift;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependentcyInjection
{
    public static void AddCoreServices(this IHostApplicationBuilder builder)
    {
        // Register repository
        builder.Services.AddScoped<IJointDriftRepository, JointDriftRepository>();
        // Register service
        builder.Services.AddScoped<JointDriftService>();
        // Register validator
        builder.Services.AddScoped<IValidator<JointDriftItem>, JointDriftItemValidator>();
        // Register AutoMapper (scan current assembly)
        builder.Services.AddAutoMapper(typeof(DependentcyInjection).Assembly);
    }
}

using EtabsExtensions.Core.ViewModels;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Register ViewModels
        services.AddTransient<MainViewModel>();

        return services;
    }
}

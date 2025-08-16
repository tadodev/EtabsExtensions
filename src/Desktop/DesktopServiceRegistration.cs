using Microsoft.Extensions.DependencyInjection;

namespace Desktop;

public static class DesktopServiceRegistration
{
    public static IServiceCollection AddDesktopServices(this IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();

        return services;
    }
}
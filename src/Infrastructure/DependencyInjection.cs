using Microsoft.Extensions.Hosting;
using System.Runtime.Versioning;
using EtabsExtensions.Core.Services;
using EtabsExtensions.Infrastructure.Data;
using EtabsExtensions.Infrastructure.Services;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=app.db";

        services.AddDbContext<TodoDbContext>(options =>
        {
            options.UseSqlite(connectionString);

            //Enable detailed errors and sensitive data logging for development
            #if DEBUG
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
            #endif
        });

        // Add other infrastructure services here
        services.AddScoped<ITodoService, TodoService>();

        return services;
    }

    public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

        try
        {
            // Ensure database is created and up-to-date
            await context.Database.EnsureCreatedAsync();

            // Apply any pending migrations
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
            }

        }
        catch (Exception e)
        {
            throw new InvalidOperationException("An error occurred while initializing the database.", e);
        }
    }


}


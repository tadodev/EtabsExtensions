using EtabsExtensions.Core.Services;
using EtabsExtensions.Infrastructure.Data;
using EtabsExtensions.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EtabsExtensions.Infrastructure;
public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=app.db";

        services.AddDbContext<TodoDbContext>(options =>
        {
            options.UseSqlite(connectionString);

            // Enable detailed errors and sensitive data logging for development
#if DEBUG
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
#endif
        });

        // Register services
        services.AddScoped<ITodoService, TodoService>();

        // Add logging
        services.AddLogging();

        return services;
    }

    public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TodoDbContext>>();

        try
        {
            logger.LogInformation("Initializing database...");

            // Ensure database is created and up-to-date
            await context.Database.EnsureCreatedAsync();

            // Apply any pending migrations
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Applying {Count} pending migrations", pendingMigrations.Count());
                await context.Database.MigrateAsync();
            }

            logger.LogInformation("Database initialization completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw new InvalidOperationException("An error occurred while initializing the database.", ex);
        }
    }

}


using EtabsExtensions.Core.ViewModels;
using EtabsExtensions.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using System.Windows;

namespace Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IHost? _host;
    private Microsoft.Extensions.Logging.ILogger? _logger;

    protected override async void OnStartup(StartupEventArgs e)
    {
        try
        {
            // Ensure logs directory exists
            Directory.CreateDirectory("logs");

            // Build configuration first
            var configuration = BuildConfiguration();

            // Configure Serilog early
            ConfigureSerilog(configuration);

            // Build host with dependency injection
            _host = CreateHost(configuration);

            // Get logger after host is built
            _logger = _host.Services.GetRequiredService<ILogger<App>>();
            _logger.LogInformation("Application starting up...");

            // Initialize database
            await InfrastructureServiceRegistration.InitializeDatabaseAsync(_host.Services);

            // Start host
            await _host.StartAsync();
            _logger.LogInformation("Host started successfully");

            // Show main window
            await ShowMainWindowAsync();

            base.OnStartup(e);
        }
        catch (Exception ex)
        {
            _logger?.LogCritical(ex, "Application failed to start");

            var errorMessage = $"Application failed to start:\n\n{ex.Message}";
            if (ex.InnerException != null)
            {
                errorMessage += $"\n\nInner Exception: {ex.InnerException.Message}";
            }

            MessageBox.Show(errorMessage, "Application Startup Error",
                MessageBoxButton.OK, MessageBoxImage.Error);

            Current.Shutdown(1);
        }
    }

    private IConfiguration BuildConfiguration()
    {
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";

        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(Environment.GetCommandLineArgs())
            .Build();
    }

    private static void ConfigureSerilog(IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "EtabsExtensions.Desktop")
            .CreateLogger();
    }

    private IHost CreateHost(IConfiguration configuration)
    {
        return Host.CreateDefaultBuilder()
            .UseSerilog() // Use Serilog as the logging provider
            .ConfigureServices((context, services) =>
            {
                // Register configuration
                services.AddSingleton(configuration);

                // Register all services
                services.AddCoreServices();
                services.AddInfrastructureServices(configuration);
                services.AddDesktopServices();
            })
            .Build();
    }

    private async Task ShowMainWindowAsync()
    {
        var mainWindow = _host!.Services.GetRequiredService<MainWindow>();
        var mainViewModel = _host.Services.GetRequiredService<MainViewModel>();

        mainWindow.DataContext = mainViewModel;
        mainWindow.Show();

        // Initialize the view model
        await mainViewModel.InitializeAsync();
        _logger!.LogInformation("Main window displayed and initialized");
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        try
        {
            _logger?.LogInformation("Application shutting down...");

            if (_host != null)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
                _host.Dispose();
                _logger?.LogInformation("Host stopped successfully");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error occurred during application shutdown");

            var errorMessage = $"Application failed to exit cleanly:\n\n{ex.Message}";
            if (ex.InnerException != null)
            {
                errorMessage += $"\n\nInner Exception: {ex.InnerException.Message}";
            }
            MessageBox.Show(errorMessage, "Application Exit Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}
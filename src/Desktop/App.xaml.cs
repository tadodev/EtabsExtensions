using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Windows;
using EtabsExtensions.Core.ViewModels;
using EtabsExtensions.Infrastructure;

namespace Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        try
        {
            // Ensure logs directory exists
            Directory.CreateDirectory("logs");


            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json",
                    optional: true)
                .AddEnvironmentVariables()
                .Build();

            

            // Build host with dependency injection
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Register configuration
                    services.AddSingleton<IConfiguration>(configuration);

                    // Register all services
                    services.AddCoreServices();
                    services.AddInfrastructureServices(configuration);
                    services.AddDesktopServices();
                })
                .Build();


            // Initialize database
            await InfrastructureServiceRegistration.InitializeDatabaseAsync(_host.Services);

            // Start host
            await _host.StartAsync();

            // Show main window
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            var mainViewModel = _host.Services.GetRequiredService<MainViewModel>();

            mainWindow.DataContext = mainViewModel;
            mainWindow.Show();

            // Initialize the view model
            await mainViewModel.InitializeAsync();

            base.OnStartup(e);
        }
        catch (Exception ex)
        {

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

    protected override async void OnExit(ExitEventArgs e)
    {
        try
        {

            if (_host != null)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
                _host.Dispose();
            }
        }
        catch (Exception ex)
        {
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
            base.OnExit(e);
        }
    }
}
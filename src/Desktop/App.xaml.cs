using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Configuration;
using System.Data;
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
                    // Register all services
                    services.AddSingleton(configuration);
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
            Log.Fatal(ex, "Application start-up failed");
            MessageBox.Show($"Application failed to start: {ex.Message}", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
            Current.Shutdown(1);
        }
    }
    protected override async void OnExit(ExitEventArgs e)
    {
        try
        {
            if (_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application exit failed");
            MessageBox.Show($"Application failed to exit: {ex.Message}", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}


using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Auto_Service.Services;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using Auto_Service.Views;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Transformation;
using Avalonia.ReactiveUI;
using Auto_Service.ViewModels;
using ReactiveUI;
using Splat;
using Microsoft.Extensions.DependencyInjection;
using Avalonia.Controls.Primitives;

namespace Auto_Service
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var authService = Locator.Current.GetService<AuthService>();
                var tokenStorageService = Locator.Current.GetService<TokenStorageService>();
                desktop.MainWindow = new LoginWindow();
                
            }

            base.OnFrameworkInitializationCompleted();
        }

        public static IServiceProvider? Services { get; private set; }

        public static void Main(string[] args)
        {
            var builder = BuildAvaloniaApp();
            
            var services = new ServiceCollection();
            services.AddSingleton<IWindowService, WindowService>();
            services.AddSingleton<IExportService, ExportService>();
            Services = services.BuildServiceProvider();

            builder.StartWithClassicDesktopLifetime(args);   
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToTrace()
                .AfterSetup(builder =>
                {
                    var services = new ServiceCollection();
                    services.AddSingleton<TokenStorageService>();
                    services.AddSingleton<AuthService>();
                    services.AddSingleton<MaintenancesService>();
                    services.AddSingleton<MasterService>();
                    services.AddSingleton<ClientService>();
                    var serviceProvider = services.BuildServiceProvider();
                });

    }
}
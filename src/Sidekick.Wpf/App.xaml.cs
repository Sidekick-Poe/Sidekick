using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sidekick.Common;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Errors;
using Sidekick.Common.Platform;
using Sidekick.Mock;
using Sidekick.Modules.Settings;
using Sidekick.Wpf.Services;

namespace Sidekick.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string APPLICATION_PROCESS_GUID = "93c46709-7db2-4334-8aa3-28d473e66041";

        public static ServiceProvider ServiceProvider { get; set; } = null!;

        private readonly ILogger<App> logger;
        private Mutex? Mutex { get; set; }

        public App()
        {
            var configurationManager = new ConfigurationManager();
            configurationManager.AddJsonFile(SidekickPaths.GetDataFilePath(SettingsService.FileName), true, true);

            var services = new ServiceCollection();
            ConfigureServices(services, configurationManager);
            ServiceProvider = services.BuildServiceProvider();
            logger = ServiceProvider.GetRequiredService<ILogger<App>>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AttachErrorHandlers();

            var viewLocator = ServiceProvider.GetRequiredService<IViewLocator>();

            if (!EnsureSingleInstance())
            {
                _ = viewLocator.Open(ErrorType.AlreadyRunning.ToUrl());
                return;
            }

            _ = viewLocator.Open("/");
        }

        private void ConfigureServices(ServiceCollection services, IConfiguration configuration)
        {
            services.AddLocalization();
            services.AddWpfBlazorWebView();

#if DEBUG
            services.AddBlazorWebViewDeveloperTools();
#endif

            services.AddSidekick(configuration);

            services.AddSingleton<IApplicationService, MockApplicationService>();
            services.AddSingleton<ITrayProvider, WpfTrayProvider>();
            services.AddSingleton<IViewLocator, WpfViewLocator>();
            services.AddSingleton(sp => (WpfViewLocator)sp.GetRequiredService<IViewLocator>());
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Mutex?.Close();
            ServiceProvider?.Dispose();
            base.OnExit(e);
        }

        private bool EnsureSingleInstance()
        {
            Mutex = new Mutex(true, APPLICATION_PROCESS_GUID, out var instanceResult);
            if (!instanceResult)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(5000);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Current.Shutdown();
                    });
                });
                return false;
            }

            return true;
        }

        private void AttachErrorHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var exception = (Exception)e.ExceptionObject;
                LogUnhandledException(exception);
            };

            DispatcherUnhandledException += (s, e) =>
            {
                LogUnhandledException(e.Exception);
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                LogUnhandledException(e.Exception);
                e.SetObserved();
            };
        }

        private void LogUnhandledException(Exception ex)
        {
            logger.LogCritical(ex, "Unhandled exception.");
            var viewLocator = ServiceProvider.GetRequiredService<IViewLocator>();
            viewLocator.Open(ErrorType.Unknown.ToUrl());
        }
    }
}

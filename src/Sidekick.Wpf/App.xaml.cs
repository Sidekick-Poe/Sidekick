using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using Sidekick.Apis.GitHub;
using Sidekick.Apis.Poe;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoePriceInfo;
using Sidekick.Apis.PoeWiki;
using Sidekick.Common;
using Sidekick.Common.Blazor;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Platform;
using Sidekick.Common.Platform.Interprocess;
using Sidekick.Common.Settings;
using Sidekick.Mock;
using Sidekick.Modules.Chat;
using Sidekick.Modules.Development;
using Sidekick.Modules.General;
using Sidekick.Modules.Maps;
using Sidekick.Modules.Wealth;
using Sidekick.Modules.Settings;
using Sidekick.Modules.Trade;
using Sidekick.Wpf.Services;

namespace Sidekick.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider { get; set; } = null!;

        private readonly ILogger<App> logger;
        private readonly ISettingsService settingsService;
        private readonly ISettings settings;
        private readonly IInterprocessService interprocessService;

        public App()
        {
            var configurationManager = new ConfigurationManager();
            configurationManager.AddJsonFile(SidekickPaths.GetDataFilePath(SettingsService.FileName), true, true);

            var services = new ServiceCollection();
            ConfigureServices(services, configurationManager);

            ServiceProvider = services.BuildServiceProvider();
            logger = ServiceProvider.GetRequiredService<ILogger<App>>();
            settingsService = ServiceProvider.GetRequiredService<ISettingsService>();
            settings = ServiceProvider.GetRequiredService<ISettings>();
            interprocessService = ServiceProvider.GetRequiredService<IInterprocessService>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var currentDirectory = Directory.GetCurrentDirectory();
            if (string.IsNullOrEmpty(settings.Current_Directory) || settings.Current_Directory != currentDirectory)
            {
                settingsService.Save("Current_Directory", currentDirectory);
                settingsService.Save("Enable_WealthTracker", false);
            }

            var viewLocator = ServiceProvider.GetRequiredService<IViewLocator>();
            if (interprocessService.IsAlreadyRunning())
            {
                if (e.Args.Length <= 0 || !e.Args[0].ToUpper().StartsWith("SIDEKICK://"))
                {
                    throw new AlreadyRunningException();
                }

                Task.Run(async () =>
                {
                    try
                    {
                        await interprocessService.SendMessage(e.Args[0]);
                    }
                    finally
                    {
                        Current.Dispatcher.Invoke(() =>
                        {
                            Current.Shutdown();
                        });
                        Environment.Exit(0);
                    }
                });
                return;
            }

            AttachErrorHandlers();
            interprocessService.StartReceiving();
            _ = viewLocator.Open("/");
        }

        private void ConfigureServices(ServiceCollection services, IConfiguration configuration)
        {
            services.AddLocalization();
#pragma warning disable CA1416 // Validate platform compatibility
            services.AddWpfBlazorWebView();
            services.AddBlazorWebViewDeveloperTools();
#pragma warning restore CA1416 // Validate platform compatibility

            services

                // MudBlazor
                .AddMudServices()
                .AddMudBlazorDialog()
                .AddMudBlazorSnackbar()
                .AddMudBlazorResizeListener()
                .AddMudBlazorScrollListener()
                .AddMudBlazorScrollManager()
                .AddMudBlazorJsApi()

                // Common
                .AddSidekickCommon(configuration)
                .AddSidekickCommonBlazor()
                .AddSidekickCommonPlatform(o =>
                {
                    o.WindowsIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/favicon.ico");
                    o.OsxIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/apple-touch-icon.png");
                })

                // Apis
                .AddSidekickGitHubApi()
                .AddSidekickPoeApi()
                .AddSidekickPoeNinjaApi()
                .AddSidekickPoePriceInfoApi()
                .AddSidekickPoeWikiApi()

                // Modules
                .AddSidekickChat()
                .AddSidekickDevelopment()
                .AddSidekickGeneral()
                .AddSidekickMaps()
                .AddSidekickSettings()
                .AddSidekickTrade()
                .AddSidekickWealth();

            services.AddSingleton<IApplicationService, MockApplicationService>();
            services.AddSingleton<ITrayProvider, WpfTrayProvider>();
            services.AddSingleton<IViewLocator, WpfViewLocator>();
            services.AddSingleton(sp => (WpfViewLocator)sp.GetRequiredService<IViewLocator>());
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ServiceProvider?.Dispose();
            base.OnExit(e);
        }

        private void AttachErrorHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var exception = (Exception)e.ExceptionObject;
                HandleException(exception);
            };

            DispatcherUnhandledException += (s, e) =>
            {
                HandleException(e.Exception);
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                HandleException(e.Exception);
                e.SetObserved();
            };
        }

        private void DeleteStaticAssets()
        {
            try
            {
                var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location);
                if (directory == null)
                {
                    return;
                }

                var path = Path.Combine(directory, "Sidekick.staticwebassets.runtime.json");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception) {}
        }

        private void HandleException(Exception ex)
        {
            logger.LogCritical(ex, "Unhandled exception.");

            if (ex is SidekickException sidekickException)
            {
                var viewLocator = ServiceProvider.GetRequiredService<IViewLocator>();
                viewLocator.Open(sidekickException.ToUrl());
            }
        }
    }
}

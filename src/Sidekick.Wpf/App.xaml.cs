using System.IO;
using System.Windows;
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
using Sidekick.Common.Database;
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
    public partial class App
    {
        public static ServiceProvider ServiceProvider { get; private set; } = null!;

        private readonly ILogger<App> logger;
        private readonly ISettingsService settingsService;
        private readonly IInterprocessService interprocessService;

        public App()
        {
#if !DEBUG
            DeleteStaticAssets();
#endif

            var services = new ServiceCollection();
            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();
            logger = ServiceProvider.GetRequiredService<ILogger<App>>();
            settingsService = ServiceProvider.GetRequiredService<ISettingsService>();
            interprocessService = ServiceProvider.GetRequiredService<IInterprocessService>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var currentDirectory = Directory.GetCurrentDirectory();
            var settingDirectory = settingsService.GetString(SettingKeys.CurrentDirectory)
                                                  .Result;
            if (string.IsNullOrEmpty(settingDirectory) || settingDirectory != currentDirectory)
            {
                settingsService
                    .Set(SettingKeys.CurrentDirectory, currentDirectory)
                    .Wait();
                settingsService
                    .Set(SettingKeys.WealthEnabled, false)
                    .Wait();
            }

            var viewLocator = ServiceProvider.GetRequiredService<IViewLocator>();
            if (interprocessService.IsAlreadyRunning())
            {
                if (e.Args.Length <= 0
                    || !e
                        .Args[0]
                        .ToUpper()
                        .StartsWith("SIDEKICK://"))
                {
                    throw new AlreadyRunningException();
                }

                Task.Run(
                    async () =>
                    {
                        try
                        {
                            await interprocessService.SendMessage(e.Args[0]);
                        }
                        finally
                        {
                            Current.Dispatcher.Invoke(
                                () =>
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

        private void ConfigureServices(ServiceCollection services)
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
                .AddSidekickCommon()
                .AddSidekickCommonBlazor()
                .AddSidekickCommonDatabase()
                .AddSidekickCommonPlatform(
                    o =>
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
            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            ServiceProvider?.Dispose();
            base.OnExit(e);
        }

        private void AttachErrorHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += (
                _,
                e) =>
            {
                var exception = (Exception)e.ExceptionObject;
                HandleException(exception);
            };

            DispatcherUnhandledException += (
                _,
                e) =>
            {
                HandleException(e.Exception);
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (
                _,
                e) =>
            {
                HandleException(e.Exception);
                e.SetObserved();
            };
        }

        // ReSharper disable once UnusedMember.Local
        private void DeleteStaticAssets()
        {
            try
            {
                var directory = Path.GetDirectoryName(
                    System.Reflection.Assembly.GetEntryAssembly()
                          ?.Location);
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
            catch
            {
                // If we fail to delete static assets, the app should not be stopped from running.
            }
        }

        private void HandleException(Exception ex)
        {
            logger.LogCritical(ex, "Unhandled exception.");

            if (ex is not SidekickException sidekickException)
            {
                sidekickException = new SidekickException("An unknown error occured. Details may be found in the Sidekick logs.");
            }

            var viewLocator = ServiceProvider.GetRequiredService<IViewLocator>();
            viewLocator.Open(sidekickException.ToUrl());
        }
    }
}

using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using MudBlazor.Services;
using Sidekick.Apis.GitHub;
using Sidekick.Apis.Poe;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoePriceInfo;
using Sidekick.Apis.PoeWiki;
using Sidekick.Common;
using Sidekick.Common.Blazor;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Errors;
using Sidekick.Common.Platform;
using Sidekick.Common.Platform.Interprocess;
using Sidekick.Mock;
using Sidekick.Modules.About;
using Sidekick.Modules.Chat;
using Sidekick.Modules.Cheatsheets;
using Sidekick.Modules.Development;
using Sidekick.Modules.General;
using Sidekick.Modules.Maps;
using Sidekick.Modules.Settings;
using Sidekick.Modules.Trade;
using Sidekick.Modules.Wealth;
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

        private IInterprocessService InterprocessService { get; set; }
        private IInterprocessClient InterprocessClient { get; set; }

        public App()
        {


            var configurationManager = new ConfigurationManager();
            try
            {
                configurationManager.AddJsonFile(SidekickPaths.GetDataFilePath(SettingsService.FileName), true, true);
            }
            catch (Exception) { }

            var services = new ServiceCollection();
            ConfigureServices(services, configurationManager);
            ServiceProvider = services.BuildServiceProvider();
            logger = ServiceProvider.GetRequiredService<ILogger<App>>();
            InterprocessService = ServiceProvider.GetRequiredService<IInterprocessService>();
            InterprocessClient = ServiceProvider.GetRequiredService<IInterprocessClient>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {

            base.OnStartup(e);

            AttachErrorHandlers();

            var viewLocator = ServiceProvider.GetRequiredService<IViewLocator>();

            if (!EnsureSingleInstance())
            {
                InterprocessClient.Start();
                InterprocessClient.CustomProtocol(e.Args);
                InterprocessClient.Dispose();

                _ = viewLocator.Open(ErrorType.AlreadyRunning.ToUrl());
                return;
            } else
            {
                InterprocessService.CustomProtocolCallback(InterprocessService_CustomProtocolCallback);
                InterprocessService.Start();
            }

            _ = viewLocator.Open("/");
        }

        public void InterprocessService_CustomProtocolCallback(string[] obj)
        {
            string messageBoxText = string.Join(", ", obj); ;
            string caption = "Initial Instance Received?";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;
            MessageBoxResult result;

            result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.OK);
        }

        private void ConfigureServices(ServiceCollection services, IConfiguration configuration)
        {
            services.AddLocalization();
            services.AddWpfBlazorWebView();

#if DEBUG
            services.AddBlazorWebViewDeveloperTools();
#endif

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
                .AddSidekickAbout()
                .AddSidekickWealth()
                .AddSidekickChat()
                .AddSidekickCheatsheets()
                .AddSidekickDevelopment()
                .AddSidekickGeneral()
                .AddSidekickMaps()
                .AddSidekickSettings(configuration)
                .AddSidekickTrade(); ;

            services.AddSingleton<IApplicationService, MockApplicationService>();
            services.AddSingleton<ITrayProvider, WpfTrayProvider>();
            services.AddSingleton<IViewLocator, WpfViewLocator>();
            services.AddSingleton(sp => (WpfViewLocator)sp.GetRequiredService<IViewLocator>());


            //string customProtocol = "SidekickProtocol";

            //var a = System.AppDomain.CurrentDomain.BaseDirectory;
            //var b = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            //var c = System.IO.Path.GetDirectoryName(b);

            //RegistryKey key = Registry.ClassesRoot.OpenSubKey(customProtocol);
            //if (key == null)
            //{
            //    key = Registry.ClassesRoot.CreateSubKey(customProtocol);
            //    key.SetValue(string.Empty, "URL: " + customProtocol);
            //    key.SetValue("URL Protocol", string.Empty);

            //    key = key.CreateSubKey(@"shell\open\command");
            //    key.SetValue(string.Empty, System.AppDomain.CurrentDomain.BaseDirectory + " " + "%1");
            //    key.Close();
            //}


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

using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Platform;
using Sidekick.Mock;
using Sidekick.Modules.Settings;

namespace Sidekick.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider;

        public App()
        {
            var configurationManager = new ConfigurationManager();
            configurationManager.AddJsonFile(SidekickPaths.GetDataFilePath(SettingsService.FileName), true, true);

            var services = new ServiceCollection();
            ConfigureServices(services, configurationManager);

            ServiceProvider = services.BuildServiceProvider();
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
            services.AddSingleton<ITrayProvider, MockTrayProvider>();
            services.AddSingleton<IViewLocator, MockViewLocator>();
        }
    }
}

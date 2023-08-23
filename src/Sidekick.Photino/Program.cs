using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Sidekick.Apis.GitHub;
using Sidekick.Apis.Poe;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoePriceInfo;
using Sidekick.Apis.PoeWiki;
using Sidekick.Common;
using Sidekick.Common.Game;
using Sidekick.Common.Platform;
using Sidekick.Mock;
using Sidekick.Modules.About;
using Sidekick.Modules.Chat;
using Sidekick.Modules.Cheatsheets;
using Sidekick.Modules.Development;
using Sidekick.Modules.General;
using Sidekick.Modules.Initialization;
using Sidekick.Modules.Maps;
using Sidekick.Modules.Settings;
using Sidekick.Modules.Trade;
using Sidekick.Modules.Update;

namespace Sidekick.Photino
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile(SidekickPaths.GetDataFilePath(SettingsService.FileName), true, true);
            var configurationRoot = configurationBuilder.Build();

            var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);

            var services = appBuilder.Services;

            services
                .AddHttpClient()
                .AddLocalization()
                .AddLogging()
                .AddSingleton<IConfiguration>(configurationRoot)

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
                .AddSidekickCommonGame()
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
                .AddSidekickChat()
                .AddSidekickCheatsheets()
                .AddSidekickDevelopment()
                .AddSidekickGeneral()
                .AddSidekickInitialization()
                .AddSidekickMaps()
                .AddSidekickSettings(configurationRoot)
                .AddSidekickTrade()
                .AddSidekickUpdate()

                // Mocks
                .AddSidekickMocks();

            // register root component and selector
            appBuilder.RootComponents.Add<Common.Blazor.Main>("app");

            var app = appBuilder.Build();

            // customize window
            app.MainWindow
                .SetIconFile("favicon.ico")
                .SetTitle("Sidekick");
            app.MainWindow.StartUrl = "/update";

            AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
            {
                // app.MainWindow.OpenAlertWindow("Fatal exception", error.ExceptionObject.ToString());
            };

            app.Run();
        }
    }
}

/*
namespace Sidekick.Photino
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config =>
                {
                    config.AddJsonFile(SidekickPaths.GetDataFilePath(SettingsService.FileName), true, true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
*/

using System;
using System.IO;
using System.Threading.Tasks;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using Sidekick.Apis.GitHub;
using Sidekick.Apis.Poe;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoePriceInfo;
using Sidekick.Apis.PoeWiki;
using Sidekick.Common;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Game;
using Sidekick.Common.Platform;
using Sidekick.Common.Platform.Tray;
using Sidekick.Electron.App;
using Sidekick.Electron.Localization;
using Sidekick.Electron.Views;
using Sidekick.Modules.About;
using Sidekick.Modules.Chat;
using Sidekick.Modules.Cheatsheets;
using Sidekick.Modules.General;
using Sidekick.Modules.Initialization;
using Sidekick.Modules.Maps;
using Sidekick.Modules.Settings;
using Sidekick.Modules.Trade;
using Sidekick.Modules.Update;

namespace Sidekick.Electron
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
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
                .AddSidekickCommonGame()
                .AddSidekickCommonPlatform(o =>
                {
                    o.WindowsIconPath = Path.GetFullPath("wwwroot/favicon.ico");
                    o.OsxIconPath = Path.GetFullPath("wwwroot/apple-touch-icon.png");
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
                .AddSidekickGeneral()
                .AddSidekickInitialization()
                .AddSidekickMaps()
                .AddSidekickSettings(configuration)
                .AddSidekickTrade()
                .AddSidekickUpdate();

            // Electron services
            services.AddTransient<TrayResources>();
            services.AddSingleton<IAppService, AppService>();
            services.AddSingleton<ElectronCookieProtection>();
            services.AddSingleton<ViewLocator>();
            services.AddSingleton<IViewLocator>((sp) => sp.GetRequiredService<ViewLocator>());
            services.AddScoped<IViewInstance, ViewInstance>();

            var mvcBuilder = services
                .AddRazorPages(options =>
                {
                    options.RootDirectory = "/ElectronPages";
                });
            services.AddServerSideBlazor();
            services.AddHttpClient();
            services.AddLocalization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ILogger<Startup> logger,
            IViewLocator viewLocator,
            ITrayProvider trayProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            //app.UseMiddleware<ElectronCookieProtectionMiddleware>();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseSidekickTray(trayProvider, viewLocator, env);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            // Electron stuff
            ElectronNET.API.Electron.NativeTheme.SetThemeSource(ThemeSourceMode.Dark);
            ElectronNET.API.Electron.WindowManager.IsQuitOnWindowAllClosed = false;

            Task.Run(async () =>
            {
                try
                {
                    // Tray trayProvider.Initialize();

                    // We need to trick Electron into thinking that our app is ready to be opened.
                    // This makes Electron hide the splashscreen. For us, it means we are ready to
                    // initialize and price check :)
                    var browserWindow = await ElectronNET.API.Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
                    {
                        Width = 1,
                        Height = 1,
                        Frame = false,
                        Show = true,
                        Transparent = true,
                        Fullscreenable = false,
                        Minimizable = false,
                        Maximizable = false,
                        SkipTaskbar = true,
                        WebPreferences = new WebPreferences()
                        {
                            NodeIntegration = false,
                        }
                    });
                    browserWindow.WebContents.OnCrashed += (killed) => ElectronNET.API.Electron.App.Exit();
                    await Task.Delay(50);
                    browserWindow.Close();

                    // Initialize Sidekick
                    await viewLocator.Open("/update");
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Could not initialize Sidekick.");
                    ElectronNET.API.Electron.App.Exit();
                }
            });
        }
    }
}

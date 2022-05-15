using System;
using System.Drawing;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using PhotinoNET;
using Sidekick.Apis.GitHub;
using Sidekick.Apis.Poe;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoePriceInfo;
using Sidekick.Common;
using Sidekick.Common.Blazor;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Game;
using Sidekick.Common.Platform;
using Sidekick.Localization;
using Sidekick.Modules.About;
using Sidekick.Modules.Cheatsheets;
using Sidekick.Modules.Initialization;
using Sidekick.Modules.Maps;
using Sidekick.Modules.Settings;
using Sidekick.Modules.Trade;
using Sidekick.Modules.Update;

namespace Sidekick.Photino
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<ErrorResources>();

            //services
            //    // MudBlazor
            //    .AddMudServices()
            //    .AddMudBlazorDialog()
            //    .AddMudBlazorSnackbar()
            //    .AddMudBlazorResizeListener()
            //    .AddMudBlazorScrollListener()
            //    .AddMudBlazorScrollManager()
            //    .AddMudBlazorJsApi()

            //    // Common
            //    .AddSidekickCommon()
            //    .AddSidekickCommonGame()
            //    .AddSidekickCommonPlatform()

            //    // Apis
            //    .AddSidekickGitHubApi()
            //    .AddSidekickPoeApi()
            //    .AddSidekickPoeNinjaApi()
            //    .AddSidekickPoePriceInfoApi()

            //    // Modules
            //    .AddSidekickAbout()
            //    .AddSidekickCheatsheets()
            //    .AddSidekickInitialization()
            //    .AddSidekickMaps()
            //    .AddSidekickSettings(configuration)
            //    .AddSidekickTrade()
            //    .AddSidekickUpdate();

            var mvcBuilder = services
               .AddRazorPages()
               .AddFluentValidation(options =>
               {
                   foreach (var module in SidekickModule.Modules)
                   {
                       options.RegisterValidatorsFromAssembly(module.Assembly);
                   }
               });
            services.AddServerSideBlazor();
            services.AddHttpClient();
            services.AddLocalization();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            Task.Run(() =>
            {
                var window = new PhotinoWindow()
                   .SetTitle("Test")
                   .SetUseOsDefaultSize(false)
                   .SetSize(new Size(600, 400))
                   .Center()
                   .Load($"wwwroot/index.html"); // Test.

                window.SetDevToolsEnabled(true);
                window.SetContextMenuEnabled(true);

                window.WaitForClose();
            });
        }

    }
}

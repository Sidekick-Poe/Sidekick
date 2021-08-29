using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;
using Sidekick.Apis.GitHub;
using Sidekick.Apis.Poe;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoePriceInfo;
using Sidekick.Common;
using Sidekick.Common.Blazor;
using Sidekick.Common.Game;
using Sidekick.Common.Platform;
using Sidekick.Mock;
using Sidekick.Modules.About;
using Sidekick.Modules.Cheatsheets;
using Sidekick.Modules.Development;
using Sidekick.Modules.Initialization;
using Sidekick.Modules.Maps;
using Sidekick.Modules.Settings;
using Sidekick.Modules.Trade;
using Sidekick.Modules.Update;
using Sidekick.Presentation.Blazor.Localization;

namespace Sidekick.Presentation.Blazor
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
            services.AddTransient<ErrorResources>();

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
                .AddSidekickCommonPlatform()

                // Apis
                .AddSidekickGitHubApi()
                .AddSidekickPoeApi()
                .AddSidekickPoeNinjaApi()
                .AddSidekickPoePriceInfoApi()

                // Modules
                .AddSidekickAbout()
                .AddSidekickCheatsheets()
                .AddSidekickDevelopment()
                .AddSidekickInitialization()
                .AddSidekickMaps()
                .AddSidekickSettings(configuration)
                .AddSidekickTrade()
                .AddSidekickUpdate()

                // Mocks
                .AddSidekickMocks();

            var mvcBuilder = services
                .AddRazorPages(options =>
                {
                    options.RootDirectory = "/Shared";
                })
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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

            app.UseMiddleware<DevelopmentStartupMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}

/*
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

            // // Common .AddSidekickCommon() .AddSidekickCommonGame() .AddSidekickCommonPlatform()

            // // Apis .AddSidekickGitHubApi() .AddSidekickPoeApi() .AddSidekickPoeNinjaApi() .AddSidekickPoePriceInfoApi()

            // // Modules .AddSidekickAbout() .AddSidekickCheatsheets() .AddSidekickInitialization()
            // .AddSidekickMaps() .AddSidekickSettings(configuration) .AddSidekickTrade() .AddSidekickUpdate();

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
*/

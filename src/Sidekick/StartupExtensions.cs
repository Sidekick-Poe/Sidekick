using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Platform.Tray;

namespace Sidekick
{
    public static class StartupExtensions
    {
        public static IApplicationBuilder UseSidekickTray(this IApplicationBuilder app, ITrayProvider trayProvider, IViewLocator viewLocator)
        {
            trayProvider.Initialize(new()
            {
                new TrayMenuItem()
                {
                    Label = "Sidekick - " + typeof(StartupExtensions).Assembly.GetName().Version.ToString(),
                    Disabled = true,
                },
                new TrayMenuItem()
                {
                    Label = "Cheatsheets",
                    OnClick = () => viewLocator.Open("/cheatsheets"),
                },
                new TrayMenuItem()
                {
                    Label = "About",
                    OnClick = () => viewLocator.Open("/about"),
                },
                new TrayMenuItem()
                {
                    Label = "Settings",
                    OnClick = () => viewLocator.Open("/settings"),
                },
                new TrayMenuItem()
                {
                    Label = "Exit",
                    OnClick = () => { System.Environment.Exit(0); return Task.CompletedTask; },
                },
            });

            return app;
        }
    }
}

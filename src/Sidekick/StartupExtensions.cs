using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Platform.Tray;

namespace Sidekick
{
    public static class StartupExtensions
    {
        public static IApplicationBuilder UseSidekickTray(this IApplicationBuilder app,
                                                          ITrayProvider trayProvider,
                                                          IViewLocator viewLocator,
                                                          IWebHostEnvironment env)
        {
            var menuItems = new List<TrayMenuItem>();

            if (env.IsDevelopment())
            {
                menuItems.Add(new()
                {
                    Label = "Send Notification",
                    OnClick = () => Task.Run(() => trayProvider.SendNotification("Test message", "Test title"))
                });
            }

            menuItems.AddRange(new List<TrayMenuItem>()
            {
                new ()
                {
                    Label = "Sidekick - " + typeof(StartupExtensions).Assembly.GetName().Version.ToString(),
                    Disabled = true,
                },
                new ()
                {
                    Label = "Cheatsheets",
                    OnClick = () => viewLocator.Open("/cheatsheets"),
                },
                new ()
                {
                    Label = "About",
                    OnClick = () => viewLocator.Open("/about"),
                },
                new ()
                {
                    Label = "Settings",
                    OnClick = () => viewLocator.Open("/settings"),
                },
                new ()
                {
                    Label = "Exit",
                    OnClick = () => { System.Environment.Exit(0); return Task.CompletedTask; },
                },
            });

            trayProvider.Initialize(menuItems);

            return app;
        }
    }
}

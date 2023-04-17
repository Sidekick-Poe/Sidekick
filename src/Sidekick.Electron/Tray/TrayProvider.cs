using System.Collections.Generic;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Extensions;
using Sidekick.Common.Platform;
using Sidekick.Common.Platform.Tray;
using Sidekick.Electron.Localization;

namespace Sidekick.Electron.Tray
{
    public class TrayProvider : ITrayProvider
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IViewLocator viewLocator;
        private readonly IClipboardProvider clipboardProvider;
        private readonly TrayResources resources;

        public TrayProvider(IWebHostEnvironment webHostEnvironment,
                            IViewLocator viewLocator,
                            IClipboardProvider clipboardProvider,
                            TrayResources resources)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.viewLocator = viewLocator;
            this.clipboardProvider = clipboardProvider;
            this.resources = resources;
        }

        public void Initialize(List<TrayMenuItem> trayMenuItems)
        {
            var menuItems = new List<MenuItem>()
            {
                new ()
                {
                    Label = resources.Title + " - " + GetType().Assembly.GetName().Version.ToString(),
                    Type = MenuType.normal,
                    Icon = $"{webHostEnvironment.ContentRootPath}Assets/16x16.png",
                    Enabled = false,
                },

                new () { Type = MenuType.separator },

                new ()
                {
                    Label = resources.Cheatsheets,
                    Click = () => { viewLocator.Open("/cheatsheets"); }
                },

                new ()
                {
                    Label = resources.About,
                    Click = () => { viewLocator.Open("/about"); }
                },

                new ()
                {
                    Label = resources.Settings,
                    Click = () => { viewLocator.Open("/settings"); }
                },

                new () { Type = MenuType.separator },

                new ()
                {
                    Label = resources.Exit,
                    Click = () => ElectronNET.API.Electron.App.Quit()
                }
            };

            if (webHostEnvironment.IsDevelopment())
            {
                menuItems.InsertRange(0, GetDevelopmentMenu());
            }

            ElectronNET.API.Electron.Tray.Show($"{webHostEnvironment.ContentRootPath}Assets/icon.png", menuItems.ToArray());
            ElectronNET.API.Electron.Tray.OnDoubleClick += (_, _) => viewLocator.Open("/settings");
            ElectronNET.API.Electron.Tray.SetToolTip(resources.Title);
        }

        public List<MenuItem> GetDevelopmentMenu()
        {
            return new List<MenuItem>()
            {
                 new () {
                    Label = "Development",
                    Type = MenuType.submenu,
                    Submenu = new MenuItem[]
                    {
                        new () {
                            Label = "Price check from clipboard",
                            Click = async () =>
                            {
                                var itemText = await clipboardProvider.GetText();
                                await viewLocator.Open($"/trade/{itemText.EncodeBase64Url()}");
                            }
                        },
                        new () {
                            Label = "Check map from clipboard",
                            Click = async () =>
                            {
                                var itemText = await clipboardProvider.GetText();
                                await viewLocator.Open($"/map/{itemText.EncodeBase64Url()}");
                            }
                        }
                    }
                },

                new () { Type = MenuType.separator },
            };
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}

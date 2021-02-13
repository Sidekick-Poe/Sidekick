using System;
using System.Collections.Generic;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Sidekick.Domain.Views;
using Sidekick.Presentation.Localization.Tray;

namespace Sidekick.Presentation.Blazor.Electron.Tray
{
    public class TrayProvider
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ILogger<TrayProvider> logger;
        private readonly IViewLocator viewLocator;

        public TrayProvider(
            IWebHostEnvironment webHostEnvironment,
            ILogger<TrayProvider> logger,
            IViewLocator viewLocator
        )
        {
            this.webHostEnvironment = webHostEnvironment;
            this.logger = logger;
            this.viewLocator = viewLocator;
        }

        public void Initialize()
        {
            try
            {
                var menuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Label = "Cheatsheets",
                        Click = () => { viewLocator.Open(View.League); }
                    },

                    new MenuItem
                    {
                        Label = "About",
                        Click = () => { viewLocator.Open(View.About); }
                    },

                    new MenuItem
                    {
                        Label = "Settings",
                        Click = () => { viewLocator.Open(View.Settings); }
                    },

                    new MenuItem
                    {
                        Label = TrayResources.Exit,
                        Click = () => ElectronNET.API.Electron.App.Quit()
                    }
                };

                ElectronNET.API.Electron.Tray.Show($"{webHostEnvironment.ContentRootPath}Assets/ExaltedOrb.png", menuItems.ToArray());
                ElectronNET.API.Electron.Tray.SetToolTip(TrayResources.Title);
            }
            catch (Exception e)
            {
                logger.LogError("Exception while initializing the tray.", e);
            }
        }
    }
}

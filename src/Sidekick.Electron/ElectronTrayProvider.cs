using ElectronNET.API.Entities;
using Sidekick.Common.Platform;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Electron
{
    public class ElectronTrayProvider : ITrayProvider
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IViewLocator viewLocator;

        public ElectronTrayProvider(
            IWebHostEnvironment webHostEnvironment,
            IViewLocator viewLocator)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.viewLocator = viewLocator;
        }

        private bool Initialized { get; set; }

        public void Initialize(List<TrayMenuItem> items)
        {
            if (Initialized)
            {
                return;
            }

            ElectronNET.API.Electron.Tray.Show(Path.Combine(webHostEnvironment.ContentRootPath, "Assets/icon.png"), items.Select(x => new MenuItem()
            {
                Label = x.Label,
                Enabled = !x.Disabled,
                Click = () =>
                {
                    if (x.OnClick != null)
                    {
                        x.OnClick();
                    }
                },
            }).ToArray());
            ElectronNET.API.Electron.Tray.OnDoubleClick += (_, _) => viewLocator.Open("/settings");
            ElectronNET.API.Electron.Tray.SetToolTip("Sidekick");

            Initialized = true;
        }
    }
}

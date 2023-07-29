using ElectronNET.API.Entities;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Platform;
using Sidekick.Common.Platform.Tray;

namespace Sidekick.Electron
{
    public class ElectronTrayProvider : ITrayProvider
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IViewLocator viewLocator;
        private readonly ILogger<ElectronTrayProvider> logger;

        public ElectronTrayProvider(IWebHostEnvironment webHostEnvironment,
            IViewLocator viewLocator,
            ILogger<ElectronTrayProvider> logger)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.viewLocator = viewLocator;
            this.logger = logger;
        }

        public void Initialize(List<TrayMenuItem> items)
        {
            ElectronNET.API.Electron.Tray.Show(Path.Combine(webHostEnvironment.ContentRootPath, "Assets/icon.png"), items.Select(x => new MenuItem()
            {
                Label = x.Label,
                Enabled = !x.Disabled,
                Click = () => x.OnClick(),
            }).ToArray());
            ElectronNET.API.Electron.Tray.OnDoubleClick += (_, _) => viewLocator.Open("/settings");
            ElectronNET.API.Electron.Tray.SetToolTip("Sidekick");
        }
    }
}

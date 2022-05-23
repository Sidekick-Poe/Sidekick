using System.Collections.Generic;
using System.Threading.Tasks;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Platform.Keybinds;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.General.Keybinds
{
    public class CloseOverlayKeybindHandler : IKeybindHandler
    {
        private readonly IViewLocator viewLocator;
        private readonly ISettings settings;

        public CloseOverlayKeybindHandler(
            IViewLocator viewLocator,
            ISettings settings)
        {
            this.viewLocator = viewLocator;
            this.settings = settings;
        }

        public List<string> GetKeybinds() => new() { settings.Key_Close };

        public bool IsValid() => viewLocator.IsOverlayOpened();

        public Task Execute(string _)
        {
            viewLocator.CloseAllOverlays();
            return Task.CompletedTask;
        }
    }
}

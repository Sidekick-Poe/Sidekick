using System.Collections.Generic;
using System.Threading.Tasks;
using Sidekick.Common;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.General.Keybinds
{
    public class CloseOverlayWithEscHandler : IKeybindHandler
    {
        private readonly IViewLocator viewLocator;
        private readonly ISettings settings;

        public CloseOverlayWithEscHandler(
            IViewLocator viewLocator,
            ISettings settings)
        {
            this.viewLocator = viewLocator;
            this.settings = settings;
        }

        public List<string> GetKeybinds() => new() { "Esc" };

        public bool IsValid() => settings.EscapeClosesOverlays && viewLocator.IsOverlayOpened();

        public Task Execute(string _)
        {
            viewLocator.CloseAllOverlays();
            return Task.CompletedTask;
        }
    }
}

using System.Threading.Tasks;
using Sidekick.Common.Blazor.Views;
using Sidekick.Domain.Keybinds;

namespace Sidekick.Electron.Keybinds
{
    public class CloseOverlayKeybindHandler : IKeybindHandler
    {
        private readonly IViewLocator viewLocator;

        public CloseOverlayKeybindHandler(IViewLocator viewLocator)
        {
            this.viewLocator = viewLocator;
        }

        public bool IsValid() => viewLocator.IsOverlayOpened();

        public Task Execute()
        {
            viewLocator.CloseAllOverlays();
            return Task.CompletedTask;
        }
    }
}

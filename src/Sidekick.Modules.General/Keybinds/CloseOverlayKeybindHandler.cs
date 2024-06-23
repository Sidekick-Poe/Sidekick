using Sidekick.Common;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Keybinds;

namespace Sidekick.Modules.General.Keybinds
{
    public class CloseOverlayKeybindHandler(
        IViewLocator viewLocator,
        ISettingsService settingsService) : IKeybindHandler
    {
        public List<string?> GetKeybinds() =>
        [
            settingsService.GetSettings()
                           .Key_Close,
        ];

        public bool IsValid(string _) => viewLocator.IsOverlayOpened();

        public Task Execute(string _)
        {
            viewLocator.CloseAllOverlays();
            return Task.CompletedTask;
        }
    }
}

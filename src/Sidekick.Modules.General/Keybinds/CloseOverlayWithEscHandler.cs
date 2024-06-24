using Sidekick.Common;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Keybinds;

namespace Sidekick.Modules.General.Keybinds
{
    public class CloseOverlayWithEscHandler(
        IViewLocator viewLocator,
        ISettingsService settingsService) : KeybindHandler
    {
        public List<string?> GetKeybinds() =>
        [
            "Esc",
        ];

        public bool IsValid(string _) => (settingsService.GetSettings().EscapeClosesOverlays ?? false) && viewLocator.IsOverlayOpened();

        public Task Execute(string _)
        {
            viewLocator.CloseAllOverlays();
            return Task.CompletedTask;
        }
    }
}

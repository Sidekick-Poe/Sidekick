using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Keybinds;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.General.Keybinds
{
    public class CloseOverlayKeybindHandler(
        IViewLocator viewLocator,
        ISettingsService settingsService) : KeybindHandler(settingsService)
    {
        private readonly ISettingsService settingsService = settingsService;

        protected override async Task<List<string?>> GetKeybinds() =>
        [
            await settingsService.GetString(SettingKeys.KeyClose)
        ];

        public override bool IsValid(string _) => viewLocator.IsOverlayOpened();

        public override Task Execute(string _)
        {
            viewLocator.CloseAllOverlays();
            return Task.CompletedTask;
        }
    }
}

using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Keybinds;
using Sidekick.Common.Platform;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.Wealth.Keybinds
{
    public class OpenWealthKeybindHandler(
        IViewLocator viewLocator,
        ISettingsService settingsService,
        IProcessProvider processProvider) : KeybindHandler(settingsService)
    {
        private readonly ISettingsService settingsService = settingsService;

        protected override async Task<List<string?>> GetKeybinds() =>
        [
            await settingsService.GetString(SettingKeys.KeyOpenWealth)
        ];

        public override bool IsValid(string _) => processProvider.IsPathOfExileInFocus || processProvider.IsSidekickInFocus;

        public override Task Execute(string _)
        {
            viewLocator.Open("/wealth");
            return Task.CompletedTask;
        }
    }
}

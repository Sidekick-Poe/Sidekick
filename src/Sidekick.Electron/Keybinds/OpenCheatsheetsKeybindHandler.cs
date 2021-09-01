using System.Threading.Tasks;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Platform;
using Sidekick.Common.Settings;
using Sidekick.Domain.Keybinds;

namespace Sidekick.Electron.Keybinds
{
    public class OpenCheatsheetsKeybindHandler : IKeybindHandler
    {
        private readonly IViewLocator viewLocator;
        private readonly ISettings settings;
        private readonly IProcessProvider processProvider;

        public OpenCheatsheetsKeybindHandler(
            IViewLocator viewLocator,
            ISettings settings,
            IProcessProvider processProvider)
        {
            this.viewLocator = viewLocator;
            this.settings = settings;
            this.processProvider = processProvider;
        }

        public bool IsValid() => processProvider.IsPathOfExileInFocus || processProvider.IsSidekickInFocus;

        public Task Execute()
        {
            viewLocator.Open($"/cheatsheets/{settings.Cheatsheets_Selected}");
            return Task.CompletedTask;
        }
    }
}

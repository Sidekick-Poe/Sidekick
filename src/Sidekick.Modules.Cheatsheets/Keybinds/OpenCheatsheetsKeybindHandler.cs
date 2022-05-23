using System.Collections.Generic;
using System.Threading.Tasks;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Platform;
using Sidekick.Common.Platform.Keybinds;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.Cheatsheets.Keybinds
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

        public List<string> GetKeybinds() => new() { settings.Cheatsheets_Key_Open };

        public bool IsValid() => processProvider.IsPathOfExileInFocus || processProvider.IsSidekickInFocus;

        public Task Execute(string _)
        {
            viewLocator.Open($"/cheatsheets/{settings.Cheatsheets_Selected}");
            return Task.CompletedTask;
        }
    }
}

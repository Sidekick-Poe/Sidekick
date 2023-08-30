using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sidekick.Common;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Platform;
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

        public bool IsValid(string _) => (processProvider.IsPathOfExileInFocus || processProvider.IsSidekickInFocus) && settings.Cheatsheets_Pages.Any();

        public Task Execute(string _)
        {
            viewLocator.Open("/cheatsheets");
            return Task.CompletedTask;
        }
    }
}

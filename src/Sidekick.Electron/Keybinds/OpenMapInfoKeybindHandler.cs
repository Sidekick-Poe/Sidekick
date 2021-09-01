using System.Threading.Tasks;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Extensions;
using Sidekick.Common.Platform;
using Sidekick.Domain.Keybinds;

namespace Sidekick.Electron.Keybinds
{
    public class OpenMapInfoKeybindHandler : IKeybindHandler
    {
        private readonly IViewLocator viewLocator;
        private readonly IClipboardProvider clipboardProvider;
        private readonly IProcessProvider processProvider;

        public OpenMapInfoKeybindHandler(
            IViewLocator viewLocator,
            IClipboardProvider clipboardProvider,
            IProcessProvider processProvider)
        {
            this.viewLocator = viewLocator;
            this.clipboardProvider = clipboardProvider;
            this.processProvider = processProvider;
        }

        public bool IsValid() => processProvider.IsPathOfExileInFocus;

        public async Task Execute()
        {
            var itemText = await clipboardProvider.Copy();
            await viewLocator.Open($"/map/{itemText.EncodeBase64Url()}");
        }
    }
}

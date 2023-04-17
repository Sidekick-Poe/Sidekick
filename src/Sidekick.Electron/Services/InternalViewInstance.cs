using System.Linq;
using ElectronNET.API;

namespace Sidekick.Electron.Services
{
    internal class InternalViewInstance
    {
        internal InternalViewInstance(BrowserWindow browser, string url)
        {
            Browser = browser;
            Key = url.Split('/').FirstOrDefault(x => !string.IsNullOrEmpty(x));
        }

        internal BrowserWindow Browser { get; }
        internal string Key { get; }
        internal bool IsOverlay { get; set; }
    }
}

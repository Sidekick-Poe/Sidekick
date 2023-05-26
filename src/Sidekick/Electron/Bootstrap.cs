using ElectronNET.API.Entities;

namespace Sidekick.Electron
{
    public class Bootstrap
    {
        public static async Task ElectronBootstrap()
        {
            Console.WriteLine("Electron Bootstrap");
            var browserWindow = await ElectronNET.API.Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
            {
                Width = 1152,
                Height = 940,
                Show = false
            });

            await browserWindow.WebContents.Session.ClearCacheAsync();

            browserWindow.OnReadyToShow += () => browserWindow.Show();
            browserWindow.SetTitle("Sidekick");
        }
    }
}

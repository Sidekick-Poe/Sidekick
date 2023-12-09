using System.Diagnostics;
using Sidekick.Common.Platform.Interprocess;
using Sidekick.Common.Settings;

namespace Sidekick.Common.Platform.Protocol
{
    public class ProtocolService
    {
        private readonly ISettingsService settingsService;
        private readonly ISettings settings;
        private readonly IInterprocessService interprocessService;

        public ProtocolService(
            ISettingsService settingsService,
            ISettings settings,
            IInterprocessService interprocessService)
        {
            this.settingsService = settingsService;
            this.settings = settings;
            this.interprocessService = interprocessService;
        }

        private TaskCompletionSource? InitializeTask { get; set; }

        public void Initialize()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            if (string.IsNullOrEmpty(settings.Current_Directory) || settings.Current_Directory != currentDirectory)
            {
                settingsService.Save("Enable_WealthTracker", false);
                settingsService.Save("Current_Directory", currentDirectory);

                ProcessStartInfo startInfo = new ProcessStartInfo(@"Sidekick.Protocol.exe");
                startInfo.Verb = "runas";
                startInfo.UseShellExecute = true;
                Process.Start(startInfo);
                interprocessService.OnMessageReceived += InterprocessService_OnMessageReceived;
            }
        }

        private void InterprocessService_OnMessageReceived(string message)
        {
            if (!message.ToUpper().StartsWith("SIDEKICK://PROTOCOL_TEST"))
            {
                return;
            }

            if (InitializeTask != null)
            {
                InitializeTask.SetResult();
            }
        }
    }
}

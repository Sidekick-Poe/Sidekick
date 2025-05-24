using System.Diagnostics;
using Microsoft.Extensions.Logging;
using PipeMethodCalls;
using PipeMethodCalls.NetJson;
using Sidekick.Common.Settings;

namespace Sidekick.Common.Platform.Windows.Interprocess;

public class InterprocessService : IInterprocessService, IDisposable
{
    private readonly ILogger<InterprocessService> logger;
    private readonly ISettingsService settingsService;
    private const string APPLICATION_PROCESS_GUID = "93c46709-7db2-4334-8aa3-28d473e66041";

    private Mutex? mutex;
    private bool isMainInstance;
    private TaskCompletionSource? installTask;

    public event Action<string>? OnMessageReceived;

    public InterprocessService(ILogger<InterprocessService> logger, ISettingsService settingsService)
    {
        this.logger = logger;
        this.settingsService = settingsService;
        InterprocessMessaging.OnMessageReceived += InterprocessMessaging_OnMessageReceived;
    }

    public async Task<bool> IsInstalled()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var settingDirectory = await settingsService.GetString(SettingKeys.InterprocessDirectory);
        var isInstalled = settingDirectory == currentDirectory;
        logger.LogInformation("[Interprocess] IsInstalled: {0}", isInstalled);
        return isInstalled;
    }

    public Task Install()
    {
        if (installTask != null) return installTask.Task;

        if (IsInstalled().Result)
        {
            logger.LogInformation("[Interprocess] Sidekick.Protocol.exe is already installed.");

            installTask = new TaskCompletionSource();
            installTask.SetResult();
            return installTask.Task;
        }

        logger.LogInformation("[Interprocess] Starting Sidekick.Protocol.exe");

        var startInfo = new ProcessStartInfo(@"Sidekick.Protocol.exe")
        {
            Verb = "runas",
            UseShellExecute = true,
        };
        Process.Start(startInfo);

        logger.LogInformation("[Interprocess] Started Sidekick.Protocol.exe");

        var currentDirectory = Directory.GetCurrentDirectory();
        settingsService.Set(SettingKeys.InterprocessDirectory, currentDirectory).Wait();

        logger.LogInformation("[Interprocess] Saved directory to settings: {0}", currentDirectory);

        installTask = new TaskCompletionSource();
        return installTask.Task;
    }

    public void StartReceiving()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                try
                {
                    using var pipeServer = new PipeServer<InterprocessMessaging>(new NetJsonPipeSerializer(), InterprocessMessaging.Pipename, () => new InterprocessMessaging());
                    await pipeServer.WaitForConnectionAsync();
                    await pipeServer.WaitForRemotePipeCloseAsync();
                }
                catch (Exception e)
                {
                    logger.LogError(e, "[Interprocess] Failed to listen for messages. Waiting 30 seconds before trying again.");
                }

                await Task.Delay(TimeSpan.FromSeconds(30));
            }
        });
    }

    public async Task SendMessage(string message)
    {
        logger.LogDebug("[Interprocess] Sending message to other application instance. {0}", message);
        using var pipeClient = new PipeClient<InterprocessMessaging>(new NetJsonPipeSerializer(), InterprocessMessaging.Pipename);
        await pipeClient.ConnectAsync();
        await pipeClient.InvokeAsync(x => InterprocessMessaging.ReceiveMessage(message));
        logger.LogDebug("[Interprocess] Message sent.");
    }

    public bool IsAlreadyRunning()
    {
        mutex ??= new Mutex(true, APPLICATION_PROCESS_GUID, out isMainInstance);
        return !isMainInstance;
    }

    private void InterprocessMessaging_OnMessageReceived(string message)
    {
        logger.LogDebug("[Interprocess] Message received from other application instance. {0}", message);
        OnMessageReceived?.Invoke(message);
    }

    public void Dispose()
    {
        mutex?.Close();
        InterprocessMessaging.OnMessageReceived -= InterprocessMessaging_OnMessageReceived;
    }
}

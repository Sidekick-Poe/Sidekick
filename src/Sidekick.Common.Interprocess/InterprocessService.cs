using Microsoft.Extensions.Logging;
using PipeMethodCalls;
using PipeMethodCalls.NetJson;

namespace Sidekick.Common.Platform.Interprocess;

public class InterprocessService : IInterprocessService, IDisposable
{
    private readonly ILogger<InterprocessService> logger;
    private const string APPLICATION_PROCESS_GUID = "93c46709-7db2-4334-8aa3-28d473e66041";

    private Mutex? mutex;
    private bool isMainInstance;

    public event Action<string>? OnMessageReceived;

    public InterprocessService(ILogger<InterprocessService> logger)
    {
        this.logger = logger;
        InterprocessMessaging.OnMessageReceived += InterprocessMessaging_OnMessageReceived;
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

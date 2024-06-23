using PipeMethodCalls;
using PipeMethodCalls.NetJson;

namespace Sidekick.Common.Platform.Interprocess
{
    public class InterprocessService : IInterprocessService, IDisposable
    {
        private const string APPLICATION_PROCESS_GUID = "93c46709-7db2-4334-8aa3-28d473e66041";

        private readonly Mutex mutex;
        private readonly bool isMainInstance;

        public event Action<string>? OnMessageReceived;

        public InterprocessService()
        {
            InterprocessMessaging.OnMessageReceived += InterprocessMessaging_OnMessageReceived;
            mutex = new Mutex(true, APPLICATION_PROCESS_GUID, out isMainInstance);
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
                    catch (Exception)
                    {
                        // We do not want to stop execution when the above fails.
                    }
                }
            });
        }

        public async Task SendMessage(string message)
        {
            using var pipeClient = new PipeClient<InterprocessMessaging>(new NetJsonPipeSerializer(), InterprocessMessaging.Pipename);
            await pipeClient.ConnectAsync();
            await pipeClient.InvokeAsync(x => x.ReceiveMessage(message));
        }

        public bool IsAlreadyRunning() => !isMainInstance;

        private void InterprocessMessaging_OnMessageReceived(string message)
        {
            OnMessageReceived?.Invoke(message);
        }

        public void Dispose()
        {
            mutex?.Close();
            InterprocessMessaging.OnMessageReceived -= InterprocessMessaging_OnMessageReceived;
        }
    }
}

using PipeMethodCalls;
using PipeMethodCalls.NetJson;

namespace Sidekick.Common.Platform.Interprocess
{
    public class InterprocessService : IInterprocessService, IDisposable
    {
        public event Action<string>? OnMessageReceived;

        public InterprocessService()
        {
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

        private void InterprocessMessaging_OnMessageReceived(string message)
        {
            OnMessageReceived?.Invoke(message);
        }

        public void Dispose()
        {
            InterprocessMessaging.OnMessageReceived -= InterprocessMessaging_OnMessageReceived;
        }
    }
}

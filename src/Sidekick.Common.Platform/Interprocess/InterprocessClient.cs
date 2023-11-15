using Microsoft.Extensions.Logging;
using PipeMethodCalls;
using PipeMethodCalls.NetJson;

namespace Sidekick.Common.Platform.Interprocess
{
    public class InterprocessClient : IInterprocessClient, IDisposable
    {
        internal PipeClient<IInterprocessService>? pipeClient;
        internal readonly ILogger<InterprocessClient> logger;

        public InterprocessClient(ILogger<InterprocessClient> logger)
        {
            this.logger = logger;
        }

        public void Start()
        {
            var pipeName = File.ReadAllText(SidekickPaths.GetDataFilePath("pipename"));

            pipeClient = new PipeClient<IInterprocessService>(
                new NetJsonPipeSerializer(),
                pipeName);

            pipeClient.ConnectAsync();
        }

        public async Task SendMessage(string[] args)
        {
            if (pipeClient == null)
            {
                return;
            }

            await pipeClient.InvokeAsync(x => x.ReceiveMessage(args));
        }

        public void Dispose()
        {
            if (pipeClient == null)
            {
                return;
            }

            pipeClient.Dispose();
        }
    }
}

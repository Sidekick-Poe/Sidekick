using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PipeMethodCalls;
using PipeMethodCalls.NetJson;
using Serilog.Core;

namespace Sidekick.Common.Platform.Interprocess
{
    public class InterprocessClient: IInterprocessClient
    {

        internal PipeClient<IInterprocessService> pipeClient;
        internal readonly ILogger<InterprocessClient> logger;

        public InterprocessClient(ILogger<InterprocessClient> logger) {
            this.logger = logger;
        }

        public void Start()
        {
      
            string pipeName = File.ReadAllText(SidekickPaths.GetDataFilePath("pipename"));

            pipeClient = new PipeClient<IInterprocessService>(
                new NetJsonPipeSerializer(),
                pipeName);

            pipeClient.ConnectAsync();
        }

        public void CustomProtocol(string[] args)
        {

            pipeClient.InvokeAsync(x => x.CustomProtocol(args));
        }

        public void Dispose()
        {
            pipeClient.Dispose();

        }
    }
}

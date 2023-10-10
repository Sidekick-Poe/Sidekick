using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PipeMethodCalls;
using PipeMethodCalls.NetJson;
using Sidekick.Common;
using static System.Net.Mime.MediaTypeNames;

namespace Sidekick.Common.Platform.Interprocess
{
    public class InterprocessService: IInterprocessService
    {
        public static event Action<string[]> OnMessage;

        internal readonly ILogger<InterprocessService> logger;

        public InterprocessService(ILogger<InterprocessService> logger)
        {
            this.logger = logger;

        }

        public void Start()
        {
            Task.Run(async () =>
            {
                PipeServer<IInterprocessService> pipeServer;

                Random random = new Random();
                while (true)
                {
                    try
                    {
                        string pipeName = "sidekick-" + random.Next(0, 1000000).ToString();

                        using (StreamWriter f = File.CreateText(SidekickPaths.GetDataFilePath("pipename")))
                        {
                            f.Write(pipeName);
                        };

                        pipeServer = new PipeServer<IInterprocessService>(
                            new NetJsonPipeSerializer(),
                            pipeName,
                            () => new InterprocessService(logger));

                        await pipeServer.WaitForConnectionAsync();

                        File.Delete(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "pipename");

                        await pipeServer.WaitForRemotePipeCloseAsync();

                        pipeServer.Dispose();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                    }

                }
            });
        }

        public void ReceiveMessage(string[] args)
        {
            OnMessage?.Invoke(args);
        }

    }
}

using Sidekick.Common.Platform.Interprocess;

namespace Sidekick.Mock
{
    public class MockInterprocessService : IInterprocessService
    {
#pragma warning disable CS0067
        public event Action<string>? OnMessageReceived;
#pragma warning disable CS0067

        public void StartReceiving()
        {
        }

        public bool IsAlreadyRunning()
        {
            return false;
        }

        public Task SendMessage(string message)
        {
            return Task.CompletedTask;
        }
    }
}

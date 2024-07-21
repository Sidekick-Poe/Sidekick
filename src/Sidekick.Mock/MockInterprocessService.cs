using Sidekick.Common.Platform.Interprocess;

namespace Sidekick.Mock
{
    public class MockInterprocessService : IInterprocessService
    {
        public event Action<string>? OnMessageReceived;

        public void StartReceiving()
        {
        }

        public bool IsAlreadyRunning()
        {
            return false;
        }

        public Task SendMessage(string message)
        {
            return Task.CompletedTask;;
        }
    }
}

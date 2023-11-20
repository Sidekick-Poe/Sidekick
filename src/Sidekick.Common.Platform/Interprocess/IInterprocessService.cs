namespace Sidekick.Common.Platform.Interprocess
{
    public interface IInterprocessService
    {
        event Action<string> OnMessageReceived;

        void StartReceiving();

        bool IsAlreadyRunning();

        Task SendMessage(string message);
    }
}

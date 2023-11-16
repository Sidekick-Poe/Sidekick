namespace Sidekick.Common.Platform.Interprocess
{
    public interface IInterprocessService
    {
        event Action<string> OnMessageReceived;

        void StartReceiving();

        Task SendMessage(string message);
    }
}

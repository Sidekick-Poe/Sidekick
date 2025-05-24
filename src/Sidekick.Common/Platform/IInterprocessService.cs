namespace Sidekick.Common.Platform;

public interface IInterprocessService
{
    event Action<string> OnMessageReceived;

    Task Install();

    void StartReceiving();

    bool IsAlreadyRunning();

    Task SendMessage(string message);
}

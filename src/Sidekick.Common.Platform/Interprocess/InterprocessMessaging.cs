namespace Sidekick.Common.Platform.Interprocess;

public class InterprocessMessaging
{
    public const string Pipename = "sidekick-pipe";

    public static event Action<string>? OnMessageReceived;

    public static void ReceiveMessage(string message)
    {
        OnMessageReceived?.Invoke(message);
    }
}

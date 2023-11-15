namespace Sidekick.Common.Platform.Interprocess
{
    public interface IInterprocessService
    {
        event Action<string[]> OnMessage;

        void ReceiveMessage(string[] args);

        void Start();
    }
}

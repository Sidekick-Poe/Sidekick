namespace Sidekick.Common.Platform.Interprocess
{
    public interface IInterprocessClient
    {
        Task SendMessage(string[] args);
        void Start();
    }
}

namespace Sidekick.Common.Platform
{
    public interface IProcessProvider
    {
        void Initialize();

        string ClientLogPath { get; }

        bool IsPathOfExileInFocus { get; }
        bool IsSidekickInFocus { get; }
    }
}

using Sidekick.Common.Initialization;

namespace Sidekick.Common.Platform
{
    public interface IProcessProvider : IInitializableService
    {
        string? ClientLogPath { get; }

        bool IsPathOfExileInFocus { get; }

        bool IsSidekickInFocus { get; }
    }
}

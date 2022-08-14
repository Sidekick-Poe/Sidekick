using System;

namespace Sidekick.Common.Platform
{
    public interface IProcessProvider
    {
        void Initialize();

        string ClientLogPath { get; }

        event Action OnFocus;

        event Action OnBlur;

        bool IsPathOfExileInFocus { get; }
        bool IsSidekickInFocus { get; }
    }
}

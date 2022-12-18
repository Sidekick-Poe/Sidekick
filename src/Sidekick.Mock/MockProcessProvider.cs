#pragma warning disable CS0067

using System;
using Sidekick.Common.Platform;

namespace Sidekick.Mock
{
    public class MockProcessProvider : IProcessProvider
    {
        public string ClientLogPath => string.Empty;

        public event Action OnFocus;

        public event Action OnBlur;

        public void Initialize()
        {
        }

        public bool IsPathOfExileInFocus => true;
        public bool IsSidekickInFocus => false;
    }
}

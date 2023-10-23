using System;
using System.Threading.Tasks;
using Sidekick.Common.Initialization;
using Sidekick.Common.Platform;

namespace Sidekick.Mock
{
    public class MockKeyboardProvider : IKeyboardProvider
    {
        public InitializationPriority Priority => InitializationPriority.Low;

#pragma warning disable CS0067

        public event Action<string> OnKeyDown;

#pragma warning restore CS0067

        public Task Initialize()
        {
            return Task.CompletedTask;
        }

        public Task PressKey(params string[] keys)
        {
            return Task.CompletedTask;
        }
    }
}

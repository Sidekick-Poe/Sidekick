using System;
using System.Threading.Tasks;
using Sidekick.Common.Initialization;
using Sidekick.Common.Platform;

namespace Sidekick.Mock
{
    public class MockKeyboardProvider : IKeyboardProvider
    {
        public InitializationPriority Priority => InitializationPriority.Low;

        public event Action<string> OnKeyDown;

        public Task Initialize()
        {
            return Task.CompletedTask;
        }

        public void PressKey(params string[] keys)
        {
        }
    }
}

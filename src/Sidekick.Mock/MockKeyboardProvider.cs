using Sidekick.Common.Platform;

namespace Sidekick.Mock
{
    public class MockKeyboardProvider : IKeyboardProvider
    {
        public int Priority => 0;

#pragma warning disable CS0067

        public event Action<string>? OnKeyDown;

#pragma warning restore CS0067

        public Task Initialize()
        {
            return Task.CompletedTask;
        }

        public void RegisterHooks()
        {
        }

        public Task PressKey(params string[] keys)
        {
            return Task.CompletedTask;
        }
    }
}

using System;
using Sidekick.Common.Platform;

namespace Sidekick.Mock
{
    public class MockKeyboardProvider : IKeyboardProvider
    {
        public event Action<string> OnKeyDown;

        public void Initialize()
        {
        }

        public void PressKey(params string[] keys)
        {
        }

        public string ToElectronAccelerator(string key)
        {
            return null;
        }
    }
}

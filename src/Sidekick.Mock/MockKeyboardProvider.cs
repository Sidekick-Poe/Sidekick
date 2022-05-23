#pragma warning disable CS0067

using System;
using Sidekick.Common.Platform;

namespace Sidekick.Mock
{
    public class MockKeyboardProvider : IKeyboardProvider
    {
        public event Action<string> OnKeyDown;

        public void Initialize()
        {
            // Do nothing in mock
        }

        public void PressKey(params string[] keys)
        {
            // Do nothing in mock
        }

        public string ToElectronAccelerator(string key)
        {
            return key;
        }
    }
}

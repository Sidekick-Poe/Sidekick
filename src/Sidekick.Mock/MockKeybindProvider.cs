using System.Collections.Generic;
using Sidekick.Common.Platform;

namespace Sidekick.Mock
{
    public class MockKeybindProvider : IKeybindProvider
    {
        public Dictionary<string, IKeybindHandler> KeybindHandlers => new();

        public void Initialize()
        {
        }
    }
}

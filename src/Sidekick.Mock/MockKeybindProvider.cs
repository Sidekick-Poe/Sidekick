using System.Collections.Generic;
using System.Threading.Tasks;
using Sidekick.Common.Initialization;
using Sidekick.Common.Keybinds;
using Sidekick.Common.Platform;

namespace Sidekick.Mock
{
    public class MockKeybindProvider : IKeybindProvider
    {
        public Dictionary<string, IKeybindHandler> KeybindHandlers => new();

        public InitializationPriority Priority => InitializationPriority.Low;

        Dictionary<string, IKeybindHandler> IKeybindProvider.KeybindHandlers => throw new System.NotImplementedException();

        public Task Initialize()
        {
            return Task.CompletedTask;
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Sidekick.Common.Platform.Keybinds;

namespace Sidekick.Common.Platform.Options
{
    /// <inheritdoc/>
    public class KeybindProvider : IKeybindProvider
    {
        private IOptionsMonitor<KeybindOptions> optionsMonitor;
        private IServiceProvider serviceProvider;

        public KeybindProvider(
            IOptionsMonitor<KeybindOptions> optionsMonitor,
            IServiceProvider serviceProvider)
        {
            this.optionsMonitor = optionsMonitor;
            this.serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public Dictionary<string, IKeybindHandler> KeybindHandlers { get; init; } = new();

        /// <inheritdoc/>
        public void Initialize()
        {
            KeybindHandlers.Clear();

            foreach (var keybindType in optionsMonitor.CurrentValue.Keybinds)
            {
                var keybindHandler = (IKeybindHandler)serviceProvider.GetService(keybindType);
                foreach (var keybind in keybindHandler.GetKeybinds())
                {
                    KeybindHandlers.Add(keybind, keybindHandler);
                }
            }
        }
    }
}

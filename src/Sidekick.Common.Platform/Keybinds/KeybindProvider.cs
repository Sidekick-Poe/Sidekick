using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sidekick.Common.Initialization;
using Sidekick.Common.Keybinds;

namespace Sidekick.Common.Platform.Options
{
    /// <inheritdoc/>
    public class KeybindProvider : IKeybindProvider
    {
        private IOptions<SidekickConfiguration> optionsMonitor;
        private IServiceProvider serviceProvider;

        public KeybindProvider(
            IOptions<SidekickConfiguration> optionsMonitor,
            IServiceProvider serviceProvider)
        {
            this.optionsMonitor = optionsMonitor;
            this.serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public Dictionary<string, IKeybindHandler> KeybindHandlers { get; init; } = new();

        public InitializationPriority Priority => InitializationPriority.Low;

        /// <inheritdoc/>
        public Task Initialize()
        {
            KeybindHandlers.Clear();

            foreach (var keybindType in optionsMonitor.Value.Keybinds)
            {
                var keybindHandler = (IKeybindHandler)serviceProvider.GetRequiredService(keybindType);
                foreach (var keybind in keybindHandler.GetKeybinds())
                {
                    KeybindHandlers.Add(keybind, keybindHandler);
                }
            }

            return Task.CompletedTask;
        }
    }
}

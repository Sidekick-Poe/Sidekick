using Microsoft.Extensions.Logging;
using Sidekick.Common;
using Sidekick.Common.Game.GameLogs;
using Sidekick.Common.Keybinds;
using Sidekick.Common.Platform;

namespace Sidekick.Modules.Chat.Keybinds
{
    public class ChatKeybindHandler(
        ISettingsService settingsService,
        IClipboardProvider clipboard,
        IKeyboardProvider keyboard,
        ILogger<ChatKeybindHandler> logger,
        IProcessProvider processProvider,
        IGameLogProvider gameLogProvider) : IKeybindHandler
    {
        private const string TokenLast = "@last";

        public List<string?> GetKeybinds() => settingsService
                                              .GetSettings()
                                              .Chat_Commands?.Select(x => x.Key)
                                              .ToList()
                                              ??
                                              [];

        public bool IsValid(string keybind) => processProvider.IsPathOfExileInFocus
                                               && (settingsService
                                                   .GetSettings()
                                                   .Chat_Commands?.Any(x => x.Key == keybind)
                                                   ?? false);

        public async Task Execute(string keybind)
        {
            var settings = settingsService.GetSettings();
            var chatCommand = settings.Chat_Commands?.FirstOrDefault(x => x.Key == keybind);
            if (chatCommand == null)
            {
                return;
            }

            var command = chatCommand.Command;
            string? clipboardValue = null;
            if (settings.RetainClipboard ?? false)
            {
                clipboardValue = await clipboard.GetText();
            }

            if (command?.Contains(TokenLast) ?? false)
            {
                var characterName = gameLogProvider.GetLatestWhisper();
                if (string.IsNullOrEmpty(characterName))
                {
                    logger.LogWarning(@"No last whisper was found in the log file.");
                    return;
                }

                command = command.Replace(TokenLast, "@" + characterName);
            }

            await clipboard.SetText(command);

            if (chatCommand.Submit)
            {
                await keyboard.PressKey(
                    "Enter",
                    "Ctrl+A",
                    "Ctrl+V",
                    "Enter",
                    "Enter",
                    "Up",
                    "Up",
                    "Esc");
            }
            else
            {
                await keyboard.PressKey("Enter", "Ctrl+A", "Ctrl+V");
            }

            if (settings.RetainClipboard ?? false)
            {
                await Task.Delay(100);
                await clipboard.SetText(clipboardValue);
            }
        }
    }
}

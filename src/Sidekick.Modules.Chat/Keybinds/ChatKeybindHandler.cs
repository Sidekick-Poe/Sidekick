using Microsoft.Extensions.Logging;
using Sidekick.Common.Game.GameLogs;
using Sidekick.Common.Keybinds;
using Sidekick.Common.Platform;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.Chat.Keybinds
{
    public class ChatKeybindHandler(
        ISettingsService settingsService,
        IClipboardProvider clipboard,
        IKeyboardProvider keyboard,
        ILogger<ChatKeybindHandler> logger,
        IProcessProvider processProvider,
        IGameLogProvider gameLogProvider) : KeybindHandler(settingsService)
    {
        private readonly ISettingsService settingsService = settingsService;

        private const string TokenLast = "@last";

        protected override async Task<List<string?>> GetKeybinds()
        {
            var chatCommands = await settingsService.GetObject<List<ChatSetting>>(SettingKeys.ChatCommands);
            return chatCommands
                   ?.Select(x => x.Key)
                   .ToList()
                   ??
                   [
                   ];
        }

        public override bool IsValid(string keybind) => processProvider.IsPathOfExileInFocus
                                               && Keybinds.Any(x => x == keybind);

        public override async Task Execute(string keybind)
        {
            var chatCommands = await settingsService.GetObject<List<ChatSetting>>(SettingKeys.ChatCommands);
            var chatCommand = chatCommands?.FirstOrDefault(x => x.Key == keybind);
            if (chatCommand == null)
            {
                return;
            }

            var command = chatCommand.Command;
            string? clipboardValue = null;
            var retainClipboard = await settingsService.GetBool(SettingKeys.RetainClipboard);
            if (retainClipboard)
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

                command = command.Replace(TokenLast, command.StartsWith("@") ? characterName : characterName);
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

            if (retainClipboard)
            {
                await Task.Delay(100);
                await clipboard.SetText(clipboardValue);
            }
        }
    }
}

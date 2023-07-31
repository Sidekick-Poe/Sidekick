using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Game.GameLogs;
using Sidekick.Common.Platform;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.Chat.Keybinds
{
    public class ChatKeybindHandler : IKeybindHandler
    {
        private const string Token_LastWhisper_CharacterName = "{LastWhisper.CharacterName}";

        private readonly ISettings settings;
        private readonly IClipboardProvider clipboard;
        private readonly IKeyboardProvider keyboard;
        private readonly ILogger<ChatKeybindHandler> logger;
        private readonly IProcessProvider processProvider;
        private readonly IGameLogProvider gameLogProvider;

        public ChatKeybindHandler(
            ISettings settings,
            IClipboardProvider clipboard,
            IKeyboardProvider keyboard,
            ILogger<ChatKeybindHandler> logger,
            IProcessProvider processProvider,
            IGameLogProvider gameLogProvider)
        {
            this.settings = settings;
            this.clipboard = clipboard;
            this.keyboard = keyboard;
            this.logger = logger;
            this.processProvider = processProvider;
            this.gameLogProvider = gameLogProvider;
        }

        public List<string> GetKeybinds() => settings.Chat_Commands.Select(x => x.Key).ToList();

        public bool IsValid() => processProvider.IsPathOfExileInFocus;

        public async Task Execute(string keybind)
        {
            var chatCommand = settings.Chat_Commands.FirstOrDefault(x => x.Key == keybind);
            if (chatCommand == null)
            {
                return;
            }

            var command = chatCommand.Command;
            string clipboardValue = null;
            if (settings.RetainClipboard)
            {
                clipboardValue = await clipboard.GetText();
            }

            if (command.Contains(Token_LastWhisper_CharacterName))
            {
                var characterName = gameLogProvider.GetLatestWhisper();
                if (string.IsNullOrEmpty(characterName))
                {
                    logger.LogWarning(@"No last whisper was found in the log file.");
                    return;
                }

                command = command.Replace(Token_LastWhisper_CharacterName, characterName);
            }

            await clipboard.SetText(command);

            if (chatCommand.Submit)
            {
                keyboard.PressKey("Enter", "Ctrl+A", "Paste", "Enter", "Enter", "Up", "Up", "Esc");
            }
            else
            {
                keyboard.PressKey("Enter", "Ctrl+A", "Paste");
            }

            if (settings.RetainClipboard)
            {
                await Task.Delay(100);
                await clipboard.SetText(clipboardValue);
            }
        }
    }
}

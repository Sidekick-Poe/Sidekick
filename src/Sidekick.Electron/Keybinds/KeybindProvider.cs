using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Platform;
using Sidekick.Common.Settings;
using Sidekick.Domain.Keybinds;

namespace Sidekick.Electron.Keybinds
{
    public class KeybindProvider : IKeybindProvider, IDisposable
    {
        private readonly ILogger<KeybindProvider> logger;
        private readonly ISettings settings;
        private readonly IKeyboardProvider keyboardProvider;
        private readonly IProcessProvider processProvider;
        private readonly IServiceProvider serviceProvider;

        public KeybindProvider(
            ILogger<KeybindProvider> logger,
            ISettings settings,
            IKeyboardProvider keyboardProvider,
            IProcessProvider processProvider,
            IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.settings = settings;
            this.keyboardProvider = keyboardProvider;
            this.processProvider = processProvider;
            this.serviceProvider = serviceProvider;
        }

        public void Initialize()
        {
            processProvider.OnFocus += Register;
            processProvider.OnBlur += Unregister;
        }

        public void Register()
        {
            Unregister();

            if (settings.EscapeClosesOverlays)
            {
                RegisterKeybind<CloseOverlayKeybindHandler>("Esc");
            }

            RegisterKeybind<CloseOverlayKeybindHandler>(settings.Key_Close);
            RegisterKeybind<FindItemKeybindHandler>(settings.Key_FindItems);
            RegisterKeybind<OpenCheatsheetsKeybindHandler>(settings.Cheatsheets_Key_Open);
            RegisterKeybind<OpenMapInfoKeybindHandler>(settings.Map_Key_Check);
            RegisterKeybind<PriceCheckItemKeybindHandler>(settings.Trade_Key_Check);
            RegisterKeybind<OpenWikiPageKeybindHandler>(settings.Wiki_Key_Open);

            foreach (var chat in settings.Chat_Commands)
            {
                RegisterChatKeybind(chat);
            }

            logger.LogDebug("[Keybind] Registered keybinds");
        }

        private void RegisterKeybind<THandler>(string keybind)
            where THandler : IKeybindHandler
        {
            if (string.IsNullOrEmpty(keybind)) return;

            var accelerator = keyboardProvider.ToElectronAccelerator(keybind);
            if (string.IsNullOrEmpty(accelerator)) return;

            ElectronNET.API.Electron.GlobalShortcut.Register(accelerator, GetKeybindAction<THandler>(accelerator, keybind));
        }

        private Action GetKeybindAction<THandler>(string accelerator, string key)
            where THandler : IKeybindHandler
        {
            return () =>
            {
                ElectronNET.API.Electron.GlobalShortcut.Unregister(accelerator);
                Thread.Sleep(1);

                var handler = serviceProvider.GetService<THandler>();
                if (handler.IsValid())
                {
                    Task.Run(async () =>
                    {
                        await handler.Execute();
                        ElectronNET.API.Electron.GlobalShortcut.Register(accelerator, GetKeybindAction<THandler>(accelerator, key));
                    });
                }
                else
                {
                    keyboardProvider.PressKey(key);
                    ElectronNET.API.Electron.GlobalShortcut.Register(accelerator, GetKeybindAction<THandler>(accelerator, key));
                }
            };
        }

        private void RegisterChatKeybind(ChatSetting chat)
        {
            if (string.IsNullOrEmpty(chat.Key)) return;

            var accelerator = keyboardProvider.ToElectronAccelerator(chat.Key);
            if (string.IsNullOrEmpty(accelerator)) return;

            ElectronNET.API.Electron.GlobalShortcut.Register(accelerator, GetChatKeybindAction(accelerator, chat));
        }

        private Action GetChatKeybindAction(string accelerator, ChatSetting chat)
        {
            return () =>
            {
                ElectronNET.API.Electron.GlobalShortcut.Unregister(accelerator);
                Thread.Sleep(1);

                var handler = serviceProvider.GetService<ChatKeybindHandler>();
                if (handler.IsValid())
                {
                    Task.Run(() => handler.Execute(chat.Command, chat.Submit));
                }
                else
                {
                    keyboardProvider.PressKey(chat.Key);
                }

                ElectronNET.API.Electron.GlobalShortcut.Register(accelerator, GetChatKeybindAction(accelerator, chat));
            };
        }

        public void Unregister()
        {
            ElectronNET.API.Electron.GlobalShortcut.UnregisterAll();
            logger.LogDebug("[Keybind] Unregistered keybinds");
        }

        public void Dispose()
        {
            processProvider.OnFocus -= Register;
            processProvider.OnBlur -= Unregister;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Sidekick.Common.Extensions;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.Settings
{
    public class SettingsModel : Common.Settings.Settings
    {
        public SettingsModel(ISettings sidekickSettings)
        {
            sidekickSettings.CopyValuesTo(this);

            // Make sure to copy by value for the chat commands. Without doing this,
            // changing the chat commands but not saving would still modify the sidekick settings.
            // We do not want that.
            Chat_Commands = sidekickSettings.Chat_Commands
                .Select(x => new ChatSetting(x.Key, x.Command, x.Submit))
                .ToList();

            Cheatsheets_Pages = sidekickSettings.Cheatsheets_Pages
                .Select(x => new CheatsheetPage(x.Name, x.Url))
                .ToList();

            WikiOptions = new Dictionary<WikiSetting, string>()
            {
                { WikiSetting.PoeWiki, "https://www.poewiki.net" },
                { WikiSetting.PoeDb, "https://poedb.tw" }
            };
        }

        public Dictionary<WikiSetting, string> WikiOptions { get; private set; }

        public Guid? CurrentKey { get; set; }
    }
}

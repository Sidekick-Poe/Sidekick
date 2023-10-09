using System;
using System.Collections.Generic;
using System.Linq;
using Sidekick.Common.Extensions;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.Settings
{
    public class SettingsModel : ISettings
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

        public bool Trade_CloseWithMouse { get; set; }

        public bool Map_CloseWithMouse { get; set; }

        public string Map_Dangerous_Regex { get; set; }

        public bool Trade_Prediction_Enable { get; set; }

        public string Trade_Key_Check { get; set; }

        public string Trade_Item_Currency { get; set; }

        public string Trade_Bulk_Currency { get; set; }

        public string Trade_Currency_PreferredMode { get; set; }

        public string Key_Close { get; set; }

        public bool EscapeClosesOverlays { get; set; }

        public string Chat_Key_Exit { get; set; }

        public string Key_FindItems { get; set; }

        public string Chat_Key_Hideout { get; set; }

        public string Chat_Key_ReplyToLastWhisper { get; set; }

        public string Chat_Key_LeaveParty { get; set; }

        public string Map_Key_Check { get; set; }

        public string Wiki_Key_Open { get; set; }

        public string Stash_Key_Left { get; set; }

        public string Stash_Key_Right { get; set; }

        public string Language_Parser { get; set; }

        public string Language_UI { get; set; }

        public string Trade_Layout { get; set; }

        public string LeagueId { get; set; }

        public string LeaguesHash { get; set; }

        public bool RetainClipboard { get; set; }

        public bool SendCrashReports { get; set; }

        public WikiSetting Wiki_Preferred { get; set; }

        public List<ChatSetting> Chat_Commands { get; set; }

        public string Cheatsheets_Selected { get; set; }
        public string Cheatsheets_Key_Open { get; set; }

        public DateTimeOffset? PoeNinja_LastClear { get; set; }

        public bool PoeWikiData_Enable { get; set; }
        public List<CheatsheetPage> Cheatsheets_Pages { get; set; }
    }
}

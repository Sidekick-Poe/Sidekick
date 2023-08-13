using System;
using System.Collections.Generic;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.Settings
{
    public class Settings : ISettings
    {
        public string Language_UI { get; set; } = "en";

        public string Language_Parser { get; set; } = "";

        public string LeagueId { get; set; } = "";

        public string LeaguesHash { get; set; } = "";

        public WikiSetting Wiki_Preferred { get; set; } = WikiSetting.PoeWiki;

        public bool RetainClipboard { get; set; } = true;

        public bool Trade_CloseWithMouse { get; set; } = false;

        public bool Map_CloseWithMouse { get; set; } = false;

        public bool Trade_Prediction_Enable { get; set; } = true;

        public bool Trade_Normalize_Values { get; set; } = true;

        // public bool SendCrashReports { get; set; } = false;

        public string Map_Dangerous_Regex { get; set; } = "reflect|regen";

        public string Trade_Layout { get; set; }

        public string Key_Close { get; set; } = "Space";

        public bool EscapeClosesOverlays { get; set; } = true;

        public string Trade_Key_Check { get; set; } = "Ctrl+D";

        public string Map_Key_Check { get; set; } = "Ctrl+X";

        public string Wiki_Key_Open { get; set; } = "Alt+W";

        public string Key_FindItems { get; set; } = "Ctrl+F";

        public List<ChatSetting> Chat_Commands { get; set; } = new()
        {
            new ChatSetting("F5", "/hideout", true),
            new ChatSetting("F4", "/leave", true),
            new ChatSetting("Ctrl+Enter", "@{LastWhisper.CharacterName} ", false),
            new ChatSetting("F9", "/exit", true),
        };

        #region Cheatsheets

        public string Cheatsheets_Key_Open { get; set; } = "F6";
        public string Cheatsheets_Selected { get; set; } = "betrayal";

        public Dictionary<string, string> Cheatsheets_WikiPages { get; set; } = new Dictionary<string, string>()
        {
            { "Betrayal", "https://www.poewiki.net/wiki/Immortal_Syndicate" },
            { "Blight", "https://www.poewiki.net/wiki/Oil" },
            { "Delve", "https://www.poewiki.net/wiki/Delve" },
            { "Heist", "https://www.poewiki.net/wiki/Heist#Chest_types" },
            { "Incursion", "https://www.poewiki.net/wiki/Incursion_room" },
            { "Vendor Recipes", "https://www.poewiki.net/wiki/Vendor_recipe_system" },
        };

        #endregion Cheatsheets

        public DateTimeOffset? PoeNinja_LastClear { get; set; } = null;

        public bool PoeWikiData_Enable { get; set; } = true;
    }
}

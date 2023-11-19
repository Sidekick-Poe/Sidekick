namespace Sidekick.Common.Settings
{
    public class Settings : ISettings
    {
        public static List<ChatSetting> GetDefaultChatCommands() =>
        new()
        {
            new ChatSetting("F5", "/hideout", true),
            new ChatSetting("F4", "/leave", true),
            new ChatSetting("Ctrl+Enter", "@last ", false),
            new ChatSetting("F9", "/exit", true),
        };

        public static List<CheatsheetPage> GetDefaultCheatsheets() =>
        new()
        {
            new CheatsheetPage("Wiki: Betrayal", "https://www.poewiki.net/wiki/Immortal_Syndicate"),
            new CheatsheetPage("Wiki: Blight", "https://www.poewiki.net/wiki/Oil"),
            new CheatsheetPage("Wiki: Delve", "https://www.poewiki.net/wiki/Delve"),
            new CheatsheetPage("Wiki: Heist", "https://www.poewiki.net/wiki/Heist"),
            new CheatsheetPage("Wiki: Incursion", "https://www.poewiki.net/wiki/Incursion_room"),
            new CheatsheetPage("Wiki: Vendor Recipes", "https://www.poewiki.net/wiki/Vendor_recipe_system"),
            new CheatsheetPage("Leveling", "https://poe-leveling.com/"),
            new CheatsheetPage("Poe Labyrinth", "https://www.poelab.com/"),
            new CheatsheetPage("Regex Tool", "https://poe.re/"),
            new CheatsheetPage("Chromatic Calculator", "https://siveran.github.io/calc.html"),
            new CheatsheetPage("Blight Anointments", "https://blight.raelys.com/"),
        };

        public string Bearer_Token { get; set; } = "";

        public DateTimeOffset? Bearer_Expiration { get; set; } = null;

        public bool Enable_WealthTracker { get; set; } = false;

        public List<string> WealthTrackerTabs { get; set; } = new();

        public int Wealth_MinimumItemTotal { get; set; } = 0;

        public string Language_UI { get; set; } = "en";

        public string Language_Parser { get; set; } = "";

        public string LeagueId { get; set; } = "";

        public string LeaguesHash { get; set; } = "";

        public WikiSetting Wiki_Preferred { get; set; } = WikiSetting.PoeWiki;

        public bool RetainClipboard { get; set; } = true;

        public bool Trade_CloseWithMouse { get; set; } = false;

        public bool Map_CloseWithMouse { get; set; } = false;

        public bool Trade_Prediction_Enable { get; set; } = true;

        public string Map_Dangerous_Regex { get; set; } = "reflect|regen";

        public string? Trade_Layout { get; set; }

        public string? Trade_Item_Currency { get; set; }

        public string Trade_Bulk_Currency { get; set; } = "divine";

        public int Trade_Bulk_MinStock { get; set; } = 5;

        public string Trade_Currency_PreferredMode { get; set; } = "Item";

        public string Key_Close { get; set; } = "Space";

        public bool EscapeClosesOverlays { get; set; } = true;

        public string Trade_Key_Check { get; set; } = "Ctrl+D";

        public string Map_Key_Check { get; set; } = "Ctrl+X";

        public string Wiki_Key_Open { get; set; } = "Alt+W";

        public string Key_FindItems { get; set; } = "Ctrl+F";

        public List<ChatSetting> Chat_Commands { get; set; } = GetDefaultChatCommands();

        #region Cheatsheets

        public string Cheatsheets_Key_Open { get; set; } = "F6";
        public string Cheatsheets_Selected { get; set; } = "betrayal";
        public List<CheatsheetPage> Cheatsheets_Pages { get; set; } = GetDefaultCheatsheets();

        #endregion Cheatsheets

        public DateTimeOffset? PoeNinja_LastClear { get; set; } = null;
    }
}

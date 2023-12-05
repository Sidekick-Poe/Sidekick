namespace Sidekick.Common.Settings
{
    public interface ISettings
    {
        string Bearer_Token { get; set; }
        DateTimeOffset? Bearer_Expiration { get; set; }
        bool Enable_WealthTracker { get; set; }
        List<string> WealthTrackerTabs { get; set; }
        int Wealth_MinimumItemTotal { get; set; }

        string Language_Parser { get; set; }
        string Language_UI { get; set; }

        string Key_Close { get; set; }
        bool EscapeClosesOverlays { get; set; }
        string LeagueId { get; set; }
        string LeaguesHash { get; set; }

        bool RetainClipboard { get; set; }
        string Key_FindItems { get; set; }

        bool Trade_CloseWithMouse { get; set; }
        string Trade_Key_Check { get; set; }
        bool Trade_Prediction_Enable { get; set; }
        string? Trade_Layout { get; set; }
        string? Trade_Item_Currency { get; set; }
        string Trade_Bulk_Currency { get; set; }
        int Trade_Bulk_MinStock { get; set; }
        string Trade_Currency_PreferredMode { get; set; }

        bool Map_CloseWithMouse { get; set; }
        string Map_Key_Check { get; set; }
        string Map_Dangerous_Regex { get; set; }

        #region Cheatsheets

        string Cheatsheets_Key_Open { get; set; }
        string Cheatsheets_Selected { get; set; }
        List<CheatsheetPage> Cheatsheets_Pages { get; set; }

        #endregion Cheatsheets

        List<ChatSetting> Chat_Commands { get; set; }

        #region Cheatsheets

        string Wiki_Key_Open { get; set; }
        WikiSetting Wiki_Preferred { get; set; }

        #endregion Cheatsheets

        DateTimeOffset? PoeNinja_LastClear { get; set; }
        string Wealth_Key_Open { get; set; }
    }
}

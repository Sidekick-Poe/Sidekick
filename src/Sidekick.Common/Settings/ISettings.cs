namespace Sidekick.Common.Settings
{
    public interface ISettings
    {
        string Language_Parser { get; }
        string Language_UI { get; }

        string Key_Close { get; }
        bool EscapeClosesOverlays { get; }
        string LeagueId { get; }
        string LeaguesHash { get; }

        bool RetainClipboard { get; }
        string Key_FindItems { get; }

        bool Trade_CloseWithMouse { get; }
        string Trade_Key_Check { get; }
        bool Trade_Prediction_Enable { get; }
        string? Trade_Layout { get; }
        string? Trade_Item_Currency { get; }
        string Trade_Bulk_Currency { get; }
        int Trade_Bulk_MinStock { get; }
        string Trade_Currency_PreferredMode { get; }

        bool Map_CloseWithMouse { get; }
        string Map_Key_Check { get; }
        string Map_Dangerous_Regex { get; }

        #region Cheatsheets

        string Cheatsheets_Key_Open { get; }
        string Cheatsheets_Selected { get; }
        List<CheatsheetPage> Cheatsheets_Pages { get; }

        #endregion Cheatsheets

        List<ChatSetting> Chat_Commands { get; }

        #region Cheatsheets

        string Wiki_Key_Open { get; }
        WikiSetting Wiki_Preferred { get; }

        #endregion Cheatsheets

        DateTimeOffset? PoeNinja_LastClear { get; }
    }
}

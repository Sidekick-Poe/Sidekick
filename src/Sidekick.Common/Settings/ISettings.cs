namespace Sidekick.Common.Settings;

public interface ISettings
{
    string Bearer_Token { get; }

    DateTimeOffset? Bearer_Expiration { get; }

    bool Enable_WealthTracker { get; }

    List<string> WealthTrackerTabs { get; }

    int Wealth_MinimumItemTotal { get; }

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

    List<ChatSetting> Chat_Commands { get; }

    string Wiki_Key_Open { get; }

    WikiSetting Wiki_Preferred { get; }

    DateTimeOffset? PoeNinja_LastClear { get; }

    string Wealth_Key_Open { get; }

    string Current_Directory { get; }
}

using System;
using System.Collections.Generic;

namespace Sidekick.Common.Settings
{
    public interface ISettings
    {
        Guid UserId { get; set; }
        string Language_Parser { get; set; }
        string Language_UI { get; set; }

        string Key_Close { get; set; }
        bool EscapeClosesOverlays { get; set; }
        string LeagueId { get; set; }
        string LeaguesHash { get; set; }

        bool RetainClipboard { get; set; }
        string Key_FindItems { get; set; }
        // bool SendCrashReports { get; set; }

        bool Trade_CloseWithMouse { get; set; }
        string Trade_Key_Check { get; set; }
        bool Trade_Prediction_Enable { get; set; }
        bool Trade_Normalize_Values { get; set; }
        string Trade_Layout { get; set; }

        bool Map_CloseWithMouse { get; set; }
        string Map_Key_Check { get; set; }
        string Map_Dangerous_Regex { get; set; }

        #region Cheatsheets

        string Cheatsheets_Key_Open { get; set; }
        string Cheatsheets_Selected { get; set; }
        string Cheatsheets_Betrayal_Sort { get; set; }

        #endregion Cheatsheets

        List<ChatSetting> Chat_Commands { get; set; }

        string Wiki_Key_Open { get; set; }
        WikiSetting Wiki_Preferred { get; set; }

        DateTimeOffset? PoeNinja_LastClear { get; set; }

        bool PoeWikiData_Enable { get; set; }
    }
}

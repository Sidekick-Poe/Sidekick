using Microsoft.Extensions.Localization;

namespace Sidekick.Common.Blazor.Settings;

public class SettingsResources(IStringLocalizer<SettingsResources> resources)
{
    public string Character_League => resources["Character_League"];
    public string General_RetainClipboard => resources["General_RetainClipboard"];
    public string Group_Keybinds => resources["Group_Keybinds"];
    public string Key_Close => resources["Key_Close"];
    public string EscapeClosesOverlays => resources["EscapeClosesOverlays"];
    public string Key_FindItems => resources["Key_FindItems"];
    public string Language_Parser => resources["Language_Parser"];
    public string Map_CloseWithMouse => resources["Map_CloseWithMouse"];
    public string Map_Dangerous_Regex => resources["Map_Dangerous_Regex"];
    public string Map_Key_Check => resources["Map_Key_Check"];
    public string PriceCheck_CloseWithMouse => resources["PriceCheck_CloseWithMouse"];
    public string PriceCheck_Key_Check => resources["PriceCheck_Key_Check"];
    public string PriceCheck_Prediction_Enable => resources["PriceCheck_Prediction_Enable"];
    public string Title => resources["Title"];
    public string Wiki_Key_Open => resources["Wiki_Key_Open"];
    public string Wiki_Preferred => resources["Wiki_Preferred"];
    public string Wealth_Key_Open => resources["Wealth_Key_Open"];
    public string WealthTracker => resources["WealthTracker"];
    public string WealthTracker_StashTabs => resources["WealthTracker_StashTabs"];
    public string WealthTracker_StashTabsInstructions => resources["WealthTracker_StashTabsInstructions"];
    public string UseInvariantForTradeResults => resources["UseInvariantForTradeResults"];
}

using Sidekick.Common.Enums;

namespace Sidekick.Common.Settings;

public static class DefaultSettings
{
    public static string LanguageParser => "en";

    public static string LanguageUi => "en";

    public static bool OpenHomeOnLaunch => true;

    public static bool SilentStart => false;

    public static string Zoom => "1";

    public static bool UseInvariantTradeResults => false;

    public static string KeyClose => "Space";

    public static string KeyOpenWealth => "";

    public static string KeyOpenPriceCheck => "Ctrl+D";

    public static string KeyOpenMapCheck => "Ctrl+X";

    public static string KeyOpenWiki => "Alt+W";

    public static string KeyFindItems => "Ctrl+F";

    public static string KeyOpenInCraftOfExile => "";

    public static bool MouseWheelNavigateStash => true;

    public static bool MouseWheelNavigateStashReverse => false;

    public static bool EscapeClosesOverlays => true;
    public static bool OverlayCloseWithMouse => false;

    public static bool RetainClipboard => true;

    public static bool WealthEnabled => false;

    public static string PreferredWiki => WikiSetting.PoeWiki.GetValueAttribute();

    public static bool PriceCheckPredictionEnabled => true;

    public static bool PriceCheckMarketEnabled => true;

    public static string MapCheckDangerousRegex => "reflect|regen";

    public static bool PriceCheckAutomaticallySearch => false;

    public static bool PriceCheckAutomaticallySearchCurrency => false;

    public static bool PriceCheckEnableAllFilters => false;

    public static string PriceCheckEnableFiltersByRegex => "";

    public static string PriceCheckItemCurrency => "";

    public static string PriceCheckBulkCurrency => "divine";

    public static int PriceCheckBulkMinimumStock => 5;

    public static string PriceCheckCurrencyMode => "item";

    public static string? PriceCheckCurrency => null;

    public static string? PriceCheckCurrencyPoE2 => null;

    public static int PriceCheckItemCurrencyMin => 0;

    public static int PriceCheckItemCurrencyMax => 0;

    public static int PriceCheckItemCurrencyMinPoE2 => 0;

    public static int PriceCheckItemCurrencyMaxPoE2 => 0;

    public static string? PriceCheckItemListedAge => null;

    public static string PriceCheckItemClassFilter => DefaultItemClassFilter.BaseType.GetValueAttribute();

    public static double PriceCheckNormalizeValue => .1;

    public static bool PriceCheckCompactMode => false;

    public static string PriceCheckStatusPoE1 => "online";

    public static string PriceCheckStatusPoE2 => "any";

    public static bool PriceCheckAutomaticallyLoadMoreData => false;

    public static List<ChatSetting> ChatCommands =>
    [
        new ChatSetting("F5", "/hideout", true),
        new ChatSetting("F4", "/leave", true),
        new ChatSetting("Ctrl+Enter", "@last ", false),
        new ChatSetting("F9", "/exit", true),
    ];

    public static bool SaveWindowPositions => false;

    public static string PriceCheckDefaultFilterType => FilterType.Minimum.GetValueAttribute();

    public static List<RegexHotkey> RegexHotkeys => [];

    public static int WealthItemTotalMinimum => 1;
}

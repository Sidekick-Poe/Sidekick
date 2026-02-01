using Sidekick.Common.Enums;

namespace Sidekick.Modules.General.Settings;

/// <summary>
///     Identifies the type of wiki
/// </summary>
public enum WikiSetting
{
    // https://www.poewiki.net
    [EnumValue("poewiki")]
    PoeWiki,

    // https://poedb.tw
    [EnumValue("poedb")]
    PoeDb,
}

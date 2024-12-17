namespace Sidekick.Common.Game.Languages.Implementations;

/// <summary>
/// This class serves the purpose of having the game localized to english, but wanting to trade on the global servers.
/// </summary>
[GameLanguage("Traditional Chinese - Global", "zh-global")]
public class GameLanguageZhGlobal : GameLanguageZhBase
{
    public override bool UseInvariantTradeResults => true;
}

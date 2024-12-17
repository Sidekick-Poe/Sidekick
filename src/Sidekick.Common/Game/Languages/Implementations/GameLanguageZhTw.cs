namespace Sidekick.Common.Game.Languages.Implementations;

/// <summary>
/// This class serves the purpose of having the game localized to english, but wanting to trade on the taiwan servers.
/// </summary>
[GameLanguage("Traditional Chinese - Taiwan", "zh")]
public class GameLanguageZhTw : GameLanguageZhBase
{
    public override bool UseInvariantTradeResults => false;
}

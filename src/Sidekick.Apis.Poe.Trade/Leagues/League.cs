using Sidekick.Apis.Poe.Items;

namespace Sidekick.Apis.Poe.Trade.Leagues;

/// <summary>
/// A Path of Exile league
/// </summary>
public class League(
    GameType game,
    string id,
    string text)
{
    /// <summary>
    /// The game this league belongs to
    /// </summary>
    public GameType Game { get; set; } = game;

    /// <summary>
    /// The identifier of the league
    /// </summary>
    public string Id { get; set; } = id;

    /// <summary>
    /// The label of the league
    /// </summary>
    public string Text { get; set; } = text;
}

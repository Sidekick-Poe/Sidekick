namespace Sidekick.Apis.Poe.Trade.Leagues;

/// <summary>
/// A Path of Exile league
/// </summary>
public class ApiLeague
{
    /// <summary>
    /// The identifier of the league
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// The label of the league
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Realm of the league
    /// </summary>
    public LeagueRealm Realm { get; set; }
}

namespace Sidekick.Apis.Poe.Leagues
{
    /// <summary>
    /// A Path of Exile league
    /// </summary>
    public class League
    {
        /// <summary>
        /// The identifier of the league
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The label of the league
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Realm of the league
        /// </summary>
        public LeagueRealm Realm { get; set; }
    }
}

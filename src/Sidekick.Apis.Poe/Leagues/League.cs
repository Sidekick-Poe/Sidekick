namespace Sidekick.Apis.Poe.Leagues
{
    /// <summary>
    /// A Path of Exile league
    /// </summary>
    public class League
    {
        public League(
            string id,
            string text)
        {
            Id = id;
            Text = text;
        }

        /// <summary>
        /// The identifier of the league
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The label of the league
        /// </summary>
        public string Text { get; set; }
    }
}

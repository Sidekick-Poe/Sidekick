namespace Sidekick.Common.Settings
{
    /// <summary>
    /// A cheatsheet page configuration.
    /// </summary>
    public class CheatsheetPage
    {
        /// <summary>
        /// A cheatsheet page configuration.
        /// </summary>
        public CheatsheetPage()
        { }

        /// <summary>
        /// A cheatsheet page configuration.
        /// </summary>
        /// <param name="name">The name of the page.</param>
        /// <param name="url">The url of the page.</param>
        public CheatsheetPage(string name, string url)
        {
            Name = name;
            Url = url;
        }

        /// <summary>
        /// Gets or sets the name of the page.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the URL of the page.
        /// </summary>
        public string? Url { get; set; }
    }
}

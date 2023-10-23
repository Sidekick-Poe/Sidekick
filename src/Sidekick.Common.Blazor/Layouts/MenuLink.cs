namespace Sidekick.Common.Blazor.Layouts
{
    /// <summary>
    /// Represents a menu link.
    /// </summary>
    public class MenuLink
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        public required string Url { get; init; }
    }
}

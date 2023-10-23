using MudBlazor;

namespace Sidekick.Common.Blazor.Layouts
{
    /// <summary>
    /// Represents a footer button.
    /// </summary>
    public class FooterAction
    {
        /// <summary>
        /// Gets or sets the name of the button.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Gets or sets the function that gets executed when the button is clicked.
        /// </summary>
        public required Func<Task> OnClick { get; init; }

        /// <summary>
        /// Gets or sets the variant of the button.
        /// </summary>
        public Variant Variant { get; init; } = Variant.Filled;

        /// <summary>
        /// Gets or sets the color of the button.
        /// </summary>
        public Color Color { get; init; } = Color.Primary;
    }
}

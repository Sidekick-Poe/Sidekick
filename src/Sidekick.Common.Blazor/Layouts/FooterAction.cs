using System;
using System.Threading.Tasks;
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
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the function that gets executed when the button is clicked.
        /// </summary>
        public Func<Task> OnClick { get; set; }

        /// <summary>
        /// Gets or sets the variant of the button.
        /// </summary>
        public Variant Variant { get; set; } = Variant.Filled;

        /// <summary>
        /// Gets or sets the color of the button.
        /// </summary>
        public Color Color { get; set; } = Color.Primary;
    }
}

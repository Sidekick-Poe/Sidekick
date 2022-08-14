using System;
using System.Threading.Tasks;

namespace Sidekick.Common.Platform.Tray
{
    /// <summary>
    /// Represents a menu item that shows up when clicking on the tray icon
    /// </summary>
    public class TrayMenuItem
    {
        /// <summary>
        /// The label of the menu item
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Func that is executed when the menu item is clicked
        /// </summary>
        public Func<Task> OnClick { get; set; }

        /// <summary>
        /// Flag to indicate if the menu item should allow to be clicked on or not
        /// </summary>
        public bool Disabled { get; set; }
    }
}

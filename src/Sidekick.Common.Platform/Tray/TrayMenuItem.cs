namespace Sidekick.Common.Platform.Tray
{
    /// <summary>
    /// Represents a menu item that shows up when clicking on the tray icon
    /// </summary>
    public class TrayMenuItem
    {
        public TrayMenuItem(string label)
        {
            Label = label;
            Disabled = true;
        }

        public TrayMenuItem(string label, Func<Task> onClick)
        {
            Label = label;
            OnClick = onClick;
        }

        /// <summary>
        /// The label of the menu item
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Func that is executed when the menu item is clicked
        /// </summary>
        public Func<Task>? OnClick { get; set; }

        /// <summary>
        /// Flag to indicate if the menu item should allow to be clicked on or not
        /// </summary>
        public bool Disabled { get; set; }
    }
}

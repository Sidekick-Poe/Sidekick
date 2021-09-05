using System.Threading.Tasks;

namespace Sidekick.Common.Settings
{
    public interface ISettingsService
    {
        /// <summary>
        /// Command to save a single setting.
        /// </summary>
        /// <param name="property">The property to update in the settings.</param>
        /// <param name="value">The value of the setting.</param>
        Task Save(string property, object value);

        /// <summary>
        /// Command to save the settings
        /// </summary>
        /// <param name="settings">The current settings to save</param>
        Task Save(ISettings settings);
    }
}

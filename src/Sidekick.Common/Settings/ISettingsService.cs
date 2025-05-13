namespace Sidekick.Common.Settings;

public interface ISettingsService
{
    /// <summary>
    /// Event when any setting is changed.
    /// </summary>
    event Action OnSettingsChanged;

    /// <summary>
    /// Gets a setting by its key.
    /// </summary>
    /// <param name="key">The key of the setting to get.</param>
    /// <returns>The value of the setting.</returns>
    Task<bool> GetBool(string key);

    /// <summary>
    /// Gets a setting by its key.
    /// </summary>
    /// <param name="key">The key of the setting to get.</param>
    /// <returns>The value of the setting.</returns>
    Task<string?> GetString(string key);

    /// <summary>
    /// Gets a setting by its key.
    /// </summary>
    /// <param name="key">The key of the setting to get.</param>
    /// <returns>The value of the setting.</returns>
    Task<int> GetInt(string key);

    /// <summary>
    /// Gets a setting by its key.
    /// </summary>
    /// <param name="key">The key of the setting to get.</param>
    /// <returns>The value of the setting.</returns>
    Task<DateTimeOffset?> GetDateTime(string key);

    /// <summary>
    /// Gets a setting by its key.
    /// </summary>
    /// <param name="key">The key of the setting to get.</param>
    /// <returns>The value of the setting.</returns>
    Task<TEnum?> GetEnum<TEnum>(string key)
        where TEnum : struct, Enum;

    /// <summary>
    /// Gets a setting by its key.
    /// </summary>
    /// <param name="key">The key of the setting to get.</param>
    /// <returns>The value of the setting.</returns>
    Task<TValue?> GetObject<TValue>(string key);

    /// <summary>
    ///     Command to save a single setting.
    /// </summary>
    /// <param name="key">The key to update in the settings.</param>
    /// <param name="value">The value of the setting.</param>
    Task Set(
        string key,
        object? value);

    /// <summary>
    /// Determines if settings are different from their default value.
    /// </summary>
    /// <returns>True if any setting is modified.</returns>
    Task<bool> IsSettingModified(params string[] keys);

    /// <summary>
    /// Restores settings to their default value by removing them.
    /// </summary>
    Task DeleteSetting(params string[] keys);
}

using Sidekick.Game;
namespace Sidekick.Common.Settings;

public interface ISettingsService
{
    Task<GameType> GetGameAsync();

    /// <summary>
    /// Event when any setting is changed.
    /// </summary>
    /// <param name="keys">The keys of the settings that have changed.</param>
    event Action<string[]>? OnSettingsChanged;

    /// <summary>
    /// Gets a setting by its key.
    /// </summary>
    /// <param name="key">The key of the setting to get.</param>
    /// <returns>The value of the setting.</returns>
    Task<bool> GetBoolAsync(string key);

    /// <summary>
    /// Gets a setting by its key without waiting, for code that can't await, like keyboard and mouse hooks.
    /// Settings are kept in memory, so this only reads the database if no setting was read before.
    /// </summary>
    /// <param name="key">The key of the setting to get.</param>
    /// <returns>The value of the setting.</returns>
    bool GetBool(string key);

    /// <summary>
    /// Gets a setting by its key.
    /// </summary>
    /// <param name="key">The key of the setting to get.</param>
    /// <returns>The value of the setting.</returns>
    Task<string?> GetStringAsync(string key);

    /// <summary>
    /// Gets a setting by its key.
    /// </summary>
    /// <param name="key">The key of the setting to get.</param>
    /// <returns>The value of the setting.</returns>
    Task<int> GetIntAsync(string key);

    /// <summary>
    /// Gets a setting by its key.
    /// </summary>
    /// <param name="key">The key of the setting to get.</param>
    /// <returns>The value of the setting.</returns>
    Task<double> GetDoubleAsync(string key);

    /// <summary>
    /// Gets a setting by its key.
    /// </summary>
    /// <param name="key">The key of the setting to get.</param>
    /// <returns>The value of the setting.</returns>
    Task<DateTimeOffset?> GetDateTimeAsync(string key);

    /// <summary>
    /// Gets a setting by its key.
    /// </summary>
    /// <param name="key">The key of the setting to get.</param>
    /// <returns>The value of the setting.</returns>
    Task<TEnum?> GetEnumAsync<TEnum>(string key)
        where TEnum : struct, Enum;

    /// <summary>
    /// Gets a setting by its key.
    /// </summary>
    /// <param name="key">The key of the setting to get.</param>
    /// <returns>The value of the setting.</returns>
    Task<TValue?> GetObjectAsync<TValue>(string key)
        where TValue : class;

    /// <summary>
    ///     Command to save a single setting.
    /// </summary>
    /// <param name="key">The key to update in the settings.</param>
    /// <param name="value">The value of the setting.</param>
    Task SetAsync(
        string key,
        object? value);

    /// <summary>
    /// Determines if settings are different from their default value.
    /// </summary>
    /// <param name="keys">The keys of the settings to check for modification.</param>
    /// <returns>True if any setting is modified.</returns>
    Task<bool> IsSettingModifiedAsync(params string[] keys);

    /// <summary>
    /// Restores settings to their default value by removing them.
    /// </summary>
    /// <param name="keys">The keys of the settings to delete.</param>
    Task DeleteSettingAsync(params string[] keys);
}

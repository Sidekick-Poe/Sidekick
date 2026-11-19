using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common.Database;
using Sidekick.Common.Database.Tables;
using Sidekick.Common.Settings.Extensions;
using Sidekick.Game;
using Sidekick.Game.Enums;

namespace Sidekick.Common.Settings;

public class SettingsService(
    DbContextOptions<SidekickDbContext> dbContextOptions,
    IOptions<SidekickConfiguration> sidekickConfiguration,
    ILogger<SettingsService> logger) : ISettingsService
{
    public event Action<string[]>? OnSettingsChanged;

    /// <summary>
    ///     Saved setting values, loaded once from the database. Only this service writes the settings table, so the cache stays in sync.
    /// </summary>
    private ConcurrentDictionary<string, string?>? cache;

    /// <summary>
    ///     Guards loading the cache and writing settings, so a write can't be lost while the cache loads.
    /// </summary>
    private readonly SemaphoreSlim cacheLock = new(1, 1);

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters =
        {
            new JsonStringEnumConverter(),
        },
    };

    public async Task<GameType> GetGame()
    {
        var leagueId = await GetString(SettingKeys.LeagueId);
        return leagueId.GetGameFromLeagueId();
    }

    public async Task<bool> GetBoolAsync(string key)
    {
        var (isSaved, savedValue) = GetSavedValue(key);
        if (isSaved && bool.TryParse(savedValue, out var boolValue))
        {
            return boolValue;
        }

        return GetDefault<bool>(key);
    }

    public bool GetBool(string key)
    {
        var settings = cache ?? LoadCache();
        if (settings.TryGetValue(key, out var savedValue) && bool.TryParse(savedValue, out var boolValue))
        {
            return boolValue;
        }

        return GetDefault<bool>(key);
    }

    public async Task<string?> GetString(string key)
    {
        var (isSaved, savedValue) = GetSavedValue(key);
        if (isSaved)
        {
            return savedValue;
        }

        return GetDefault<string>(key);
    }

    public async Task<int> GetInt(string key)
    {
        var (isSaved, savedValue) = GetSavedValue(key);
        if (isSaved && int.TryParse(savedValue, out var intValue))
        {
            return intValue;
        }

        return GetDefault<int>(key);
    }

    public async Task<double> GetDouble(string key)
    {
        var (isSaved, savedValue) = GetSavedValue(key);
        if (isSaved && double.TryParse(savedValue, out var doubleValue))
        {
            return doubleValue;
        }

        return GetDefault<double>(key);
    }

    public async Task<DateTimeOffset?> GetDateTime(string key)
    {
        var (isSaved, savedValue) = GetSavedValue(key);
        if (isSaved && DateTimeOffset.TryParse(savedValue, out var dateTimeValue))
        {
            return dateTimeValue;
        }

        return GetDefault<DateTimeOffset>(key);
    }

    public async Task<TValue?> GetObject<TValue>(string key)
        where TValue : class
    {
        var (isSaved, savedValue) = GetSavedValue(key);
        if (isSaved)
        {
            try
            {
                return JsonSerializer.Deserialize<TValue>(savedValue ?? string.Empty, JsonSerializerOptions) ?? null;
            }
            catch (Exception e)
            {
                logger.LogError(e, "[SettingsService] Could not deserialize the value to the requested object.");
            }
        }

        var defaultValue = GetDefault<TValue>(key);
        if (defaultValue != null)
        {
            // We need to make sure that the object returned here is of a different instance than the default.
            var serializedDefault = JsonSerializer.Serialize(defaultValue, JsonSerializerOptions);
            defaultValue = JsonSerializer.Deserialize<TValue>(serializedDefault, JsonSerializerOptions);
        }

        return defaultValue ?? null;
    }

    public async Task<TEnum?> GetEnum<TEnum>(string key)
        where TEnum : struct, Enum
    {
        var (isSaved, savedValue) = GetSavedValue(key);
        if (isSaved && Enum.TryParse<TEnum>(savedValue, out var enumValue))
        {
            return enumValue;
        }

        var enumFromAttribute = isSaved ? savedValue.GetEnumFromValue<TEnum>() : (TEnum?)null;
        if (enumFromAttribute != null)
        {
            return enumFromAttribute;
        }

        if (!sidekickConfiguration.Value.DefaultSettings.TryGetValue(key, out var defaultValue))
        {
            return null;
        }

        try
        {
            if (Enum.TryParse<TEnum>(defaultValue.ToString(), out var defaultEnumValue))
            {
                return defaultEnumValue;
            }

            var defaultEnumValueFromAttribute = defaultValue.ToString()?.GetEnumFromValue<TEnum>();
            if (defaultEnumValueFromAttribute != null)
            {
                return defaultEnumValueFromAttribute;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "[SettingsService] Could not cast the default setting value to the requested object.");
            throw;
        }

        return null;
    }

    public async Task Set(
        string key,
        object? value)
    {
        var stringValue = GetStringValue(value);

        var defaultConfiguration = sidekickConfiguration.Value.DefaultSettings.GetValueOrDefault(key);
        defaultConfiguration ??= GetDefaultValue(value);
        if (defaultConfiguration != null)
        {
            var defaultValue = GetStringValue(defaultConfiguration);
            if (defaultValue == stringValue) stringValue = null;
        }

        var settings = cache ?? LoadCache();
        await cacheLock.WaitAsync();
        try
        {
            await using var dbContext = new SidekickDbContext(dbContextOptions);
            var dbSetting = await dbContext
                .Settings.Where(x => x.Key == key)
                .FirstOrDefaultAsync();
            if (stringValue == null)
            {
                if (dbSetting == null)
                {
                    return;
                }

                dbContext.Settings.Remove(dbSetting);
                await dbContext.SaveChangesAsync();
                settings.TryRemove(key, out _);
            }
            else
            {
                if (dbSetting == null)
                {
                    dbSetting = new Setting()
                    {
                        Key = key,
                        Value = stringValue,
                    };
                    dbContext.Add(dbSetting);
                }
                else
                {
                    dbSetting.Value = stringValue;
                }

                await dbContext.SaveChangesAsync();
                settings[key] = stringValue;
            }
        }
        finally
        {
            cacheLock.Release();
        }

        OnSettingsChanged?.Invoke([key]);
    }

    public async Task<bool> IsSettingModified(params string[] keys)
    {
        if (keys.Length == 0) return false;

        var settings = cache ?? LoadCache();
        return keys.Any(settings.ContainsKey);
    }

    public async Task DeleteSetting(params string[] keys)
    {
        if (keys.Length == 0) return;

        var settings = cache ?? LoadCache();
        await cacheLock.WaitAsync();
        try
        {
            await using var dbContext = new SidekickDbContext(dbContextOptions);
            var dbSettings = await dbContext.Settings.Where(x => keys.Contains(x.Key)).ToListAsync();
            if (dbSettings.Count == 0)
            {
                return;
            }

            dbContext.Settings.RemoveRange(dbSettings);
            await dbContext.SaveChangesAsync();
            foreach (var dbSetting in dbSettings)
            {
                settings.TryRemove(dbSetting.Key, out _);
            }
        }
        finally
        {
            cacheLock.Release();
        }

        OnSettingsChanged?.Invoke(keys);
    }

    private (bool IsSaved, string? Value) GetSavedValue(string key)
    {
        var settings = cache ?? LoadCache();
        var isSaved = settings.TryGetValue(key, out var value);
        return (isSaved, value);
    }

    private ConcurrentDictionary<string, string?> LoadCache()
    {
        cacheLock.Wait();
        try
        {
            if (cache == null)
            {
                using var dbContext = new SidekickDbContext(dbContextOptions);
                cache = new ConcurrentDictionary<string, string?>(dbContext.Settings.ToDictionary(x => x.Key, x => x.Value));
            }

            return cache;
        }
        finally
        {
            cacheLock.Release();
        }
    }

    private static string? GetStringValue(object? value)
    {
        if (value == null)
        {
            return null;
        }

        return value switch
        {
            double x => x.ToString(CultureInfo.InvariantCulture),
            int x => x.ToString(),
            DateTimeOffset x => x.ToString(),
            Enum x => x.GetValueAttribute(),
            string x => x,
            _ => JsonSerializer.Serialize(value, JsonSerializerOptions),
        };
    }

    private TValue? GetDefault<TValue>(string key)
    {
        if (sidekickConfiguration.Value.DefaultSettings.TryGetValue(key, out var value) && value is TValue typedValue)
        {
            return typedValue;
        }

        return default;
    }

    private static object? GetDefaultValue(object? value)
    {
        return value switch
        {
            bool => false,
            string => string.Empty,
            int => 0,
            double => 0,
            _ => null,
        };
    }
}

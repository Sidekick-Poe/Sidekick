using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sidekick.Common.Database;
using Sidekick.Common.Database.Tables;
using Sidekick.Common.Enums;

namespace Sidekick.Common.Settings;

public class SettingsService(
    DbContextOptions<SidekickDbContext> dbContextOptions,
    IOptions<SidekickConfiguration> sidekickConfiguration,
    ILogger<SettingsService> logger) : ISettingsService
{
    public event Action<string[]>? OnSettingsChanged;

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters =
        {
            new JsonStringEnumConverter()
        },
    };

    public async Task<bool> GetBool(string key)
    {
        await using var dbContext = new SidekickDbContext(dbContextOptions);
        var dbSetting = await dbContext
            .Settings.Where(x => x.Key == key)
            .FirstOrDefaultAsync();
        if (dbSetting != null && bool.TryParse(dbSetting.Value, out var boolValue))
        {
            return boolValue;
        }

        return GetDefault<bool>(key);
    }

    public async Task<string?> GetString(string key)
    {
        await using var dbContext = new SidekickDbContext(dbContextOptions);
        var dbSetting = await dbContext
            .Settings.Where(x => x.Key == key)
            .FirstOrDefaultAsync();
        if (dbSetting != null)
        {
            return dbSetting.Value;
        }

        return GetDefault<string>(key);
    }

    public async Task<int> GetInt(string key)
    {
        await using var dbContext = new SidekickDbContext(dbContextOptions);
        var dbSetting = await dbContext
            .Settings.Where(x => x.Key == key)
            .FirstOrDefaultAsync();
        if (dbSetting != null && int.TryParse(dbSetting.Value, out var intValue))
        {
            return intValue;
        }

        return GetDefault<int>(key);
    }

    public async Task<double> GetDouble(string key)
    {
        await using var dbContext = new SidekickDbContext(dbContextOptions);
        var dbSetting = await dbContext
            .Settings.Where(x => x.Key == key)
            .FirstOrDefaultAsync();
        if (dbSetting != null && double.TryParse(dbSetting.Value, out var doubleValue))
        {
            return doubleValue;
        }

        return GetDefault<double>(key);
    }

    public async Task<DateTimeOffset?> GetDateTime(string key)
    {
        await using var dbContext = new SidekickDbContext(dbContextOptions);
        var dbSetting = await dbContext
            .Settings.Where(x => x.Key == key)
            .FirstOrDefaultAsync();
        if (dbSetting != null && DateTimeOffset.TryParse(dbSetting.Value, out var dateTimeValue))
        {
            return dateTimeValue;
        }

        return GetDefault<DateTimeOffset>(key);
    }

    public async Task<TValue?> GetObject<TValue>(string key)
        where TValue : class
    {
        await using var dbContext = new SidekickDbContext(dbContextOptions);
        var dbSetting = await dbContext
            .Settings.Where(x => x.Key == key)
            .FirstOrDefaultAsync();
        if (dbSetting != null)
        {
            try
            {
                return JsonSerializer.Deserialize<TValue>(dbSetting.Value ?? string.Empty, JsonSerializerOptions) ?? null;
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
        await using var dbContext = new SidekickDbContext(dbContextOptions);
        var dbSetting = await dbContext
            .Settings.Where(x => x.Key == key)
            .FirstOrDefaultAsync();
        if (dbSetting != null && Enum.TryParse<TEnum>(dbSetting.Value, out var enumValue))
        {
            return enumValue;
        }

        var enumFromAttribute = dbSetting?.Value.GetEnumFromValue<TEnum>();
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
            OnSettingsChanged?.Invoke([key]);
            return;
        }

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
        OnSettingsChanged?.Invoke([key]);
    }

    public async Task<bool> IsSettingModified(params string[] keys)
    {
        if (keys.Length == 0) return false;

        await using var dbContext = new SidekickDbContext(dbContextOptions);
        return await dbContext.Settings.Where(x => keys.Contains(x.Key)).AnyAsync();
    }

    public async Task DeleteSetting(params string[] keys)
    {
        if (keys.Length == 0) return;

        await using var dbContext = new SidekickDbContext(dbContextOptions);
        var changed = false;

        var dbSettings = await dbContext.Settings.Where(x => keys.Contains(x.Key)).ToListAsync();

        foreach (var key in keys)
        {
            var dbSetting = dbSettings.FirstOrDefault(x => x.Key == key);

            if (dbSetting != null)
            {
                dbContext.Settings.Remove(dbSetting);
                changed = true;
            }
        }

        if (changed)
        {
            await dbContext.SaveChangesAsync();
            OnSettingsChanged?.Invoke(keys);
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

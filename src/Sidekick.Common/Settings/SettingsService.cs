using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Database;
using Sidekick.Common.Database.Tables;
using Sidekick.Common.Enums;

namespace Sidekick.Common.Settings;

public class SettingsService(
    DbContextOptions<SidekickDbContext> dbContextOptions,
    ILogger<SettingsService> logger) : ISettingsService
{
    public event Action<string[]>? OnSettingsChanged;

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

        var defaultProperty = typeof(DefaultSettings).GetProperty(key);
        if (defaultProperty == null)
        {
            return false;
        }

        return (bool)(defaultProperty.GetValue(null) ?? false);
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

        var defaultProperty = typeof(DefaultSettings).GetProperty(key);
        if (defaultProperty == null)
        {
            return null;
        }

        return (string?)(defaultProperty.GetValue(null) ?? null);
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

        var defaultProperty = typeof(DefaultSettings).GetProperty(key);
        if (defaultProperty == null)
        {
            return 0;
        }

        return (int)(defaultProperty.GetValue(null) ?? 0);
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

        var defaultProperty = typeof(DefaultSettings).GetProperty(key);
        if (defaultProperty == null)
        {
            return 0;
        }

        return (double)(defaultProperty.GetValue(null) ?? 0);
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

        var defaultProperty = typeof(DefaultSettings).GetProperty(key);
        if (defaultProperty == null)
        {
            return null;
        }

        return (DateTimeOffset?)(defaultProperty.GetValue(null) ?? null);
    }

    public async Task<TValue> GetObject<TValue>(string key, Func<TValue> defaultFunc)
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
                return JsonSerializer.Deserialize<TValue>(dbSetting.Value ?? string.Empty) ?? defaultFunc.Invoke();
            }
            catch (Exception e)
            {
                logger.LogError(e, "[SettingsService] Could not deserialize the value to the requested object.");
            }
        }

        var defaultProperty = typeof(DefaultSettings).GetProperty(key);
        if (defaultProperty != null)
        {
            try
            {
                return (TValue)(defaultProperty.GetValue(null) ?? throw new Exception("The default settings returned null."));
            }
            catch (Exception e)
            {
                logger.LogError(e, "[SettingsService] Could not cast the default setting value to the requested object.");
            }
        }

        return defaultFunc.Invoke();
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

        var defaultProperty = typeof(DefaultSettings).GetProperty(key);
        if (defaultProperty == null)
        {
            return null;
        }

        try
        {
            var propertyValue = defaultProperty.GetValue(null)?.ToString();
            if (Enum.TryParse<TEnum>(propertyValue, out var defaultValue))
            {
                return defaultValue;
            }

            var defaultEnumValueFromAttribute = propertyValue?.GetEnumFromValue<TEnum>();
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

        var defaultProperty = typeof(DefaultSettings).GetProperty(key);
        if (defaultProperty != null)
        {
            var defaultValue = GetStringValue(defaultProperty.GetValue(null));
            if (defaultValue == stringValue)
            {
                stringValue = null;
            }
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
        if (keys.Length == 0)
        {
            return false;
        }

        await using var dbContext = new SidekickDbContext(dbContextOptions);

        var dbSettings = await dbContext.Settings.Where(x => keys.Contains(x.Key)).ToListAsync();

        foreach (var key in keys)
        {
            var dbSetting = dbSettings.FirstOrDefault(x => x.Key == key);

            if (dbSetting == null)
            {
                continue;
            }

            var defaultProperty = typeof(DefaultSettings).GetProperty(key);
            if (defaultProperty == null)
            {
                continue;
            }

            var defaultValue = GetStringValue(defaultProperty.GetValue(null));

            if (defaultValue != dbSetting.Value)
            {
                return true;
            }
        }

        return false;
    }

    public async Task DeleteSetting(params string[] keys)
    {
        if (keys.Length == 0)
        {
            return;
        }

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
            _ => JsonSerializer.Serialize(value),
        };
    }
}

using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Database;
using Sidekick.Common.Database.Tables;
using Sidekick.Common.Enums;

namespace Sidekick.Common.Settings
{
    public class SettingsService(
        DbContextOptions<SidekickDbContext> dbContextOptions,
        ILogger<SettingsService> logger) : ISettingsService
    {
        public event Action? OnSettingsChanged;

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
                return default;
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
                return default;
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
                return default;
            }

            return (int)(defaultProperty.GetValue(null) ?? 0);
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
                return default;
            }

            return (DateTimeOffset?)(defaultProperty.GetValue(null) ?? null);
        }

        public async Task<TValue?> GetObject<TValue>(string key)
        {
            await using var dbContext = new SidekickDbContext(dbContextOptions);
            var dbSetting = await dbContext
                                  .Settings.Where(x => x.Key == key)
                                  .FirstOrDefaultAsync();
            if (dbSetting != null)
            {
                try
                {
                    return JsonSerializer.Deserialize<TValue>(dbSetting.Value ?? string.Empty);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "[SettingsService] Could not deserialize the value to the requested object.");
                }
            }

            var defaultProperty = typeof(DefaultSettings).GetProperty(key);
            if (defaultProperty == null)
            {
                return default;
            }

            try
            {
                return (TValue?)(defaultProperty.GetValue(null) ?? null);
            }
            catch (Exception e)
            {
                logger.LogError(e, "[SettingsService] Could not cast the default setting value to the requested object.");
                throw;
            }
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
                return default;
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

            return default;
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

            OnSettingsChanged?.Invoke();
        }

        private string? GetStringValue(object? value)
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
}

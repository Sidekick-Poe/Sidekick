using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Sidekick.Common;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.Poe.Tests.Mocks;

public class TestSettingsService(IOptions<SidekickConfiguration> configuration) : ISettingsService
{
    public event Action<string[]>? OnSettingsChanged;
    private readonly Dictionary<string, string?> store = new();

    public Task<bool> GetBool(string key)
    {
        if (store.TryGetValue(key, out var value) && bool.TryParse(value, out var boolValue))
        {
            return Task.FromResult(boolValue);
        }

        return Task.FromResult(GetDefault<bool>(key));
    }

    public Task<string?> GetString(string key)
    {
        if (store.TryGetValue(key, out var value))
        {
            return Task.FromResult(value);
        }

        return Task.FromResult(GetDefault<string>(key));
    }

    public Task<int> GetInt(string key)
    {
        if (store.TryGetValue(key, out var value) && int.TryParse(value, out var intValue))
        {
            return Task.FromResult(intValue);
        }

        return Task.FromResult(GetDefault<int>(key));
    }

    public Task<double> GetDouble(string key)
    {
        if (store.TryGetValue(key, out var value) && double.TryParse(value, out var intValue))
        {
            return Task.FromResult(intValue);
        }

        return Task.FromResult(GetDefault<double>(key));
    }

    public Task<DateTimeOffset?> GetDateTime(string key)
    {
        if (store.TryGetValue(key, out var value) && DateTimeOffset.TryParse(value, out var dateValue))
        {
            return Task.FromResult<DateTimeOffset?>(dateValue);
        }

        return Task.FromResult<DateTimeOffset?>(GetDefault<DateTimeOffset>(key));
    }

    public Task<TEnum?> GetEnum<TEnum>(string key) where TEnum : struct, Enum
    {
        if (store.TryGetValue(key, out var value))
        {
            if (Enum.TryParse<TEnum>(value, out var enumValue))
            {
                return Task.FromResult<TEnum?>(enumValue);
            }

            var enumFromAttribute = value?.GetEnumFromValue<TEnum>();
            if (enumFromAttribute != null)
            {
                return Task.FromResult(enumFromAttribute);
            }
        }

        if (!configuration.Value.DefaultSettings.TryGetValue(key, out var defaultValue))
        {
            return Task.FromResult<TEnum?>(null);
        }

        if (Enum.TryParse<TEnum>(defaultValue.ToString(), out var defaultEnumValue))
        {
            return Task.FromResult<TEnum?>(defaultEnumValue);
        }

        var defaultEnumValueFromAttribute = defaultValue.ToString()?.GetEnumFromValue<TEnum>();
        if (defaultEnumValueFromAttribute != null)
        {
            return Task.FromResult(defaultEnumValueFromAttribute);
        }

        return Task.FromResult<TEnum?>(null);
    }

    public Task<TValue?> GetObject<TValue>(string key)
        where TValue : class
    {
        if (store.TryGetValue(key, out var value))
        {
            try
            {
                if (!string.IsNullOrEmpty(value)) return Task.FromResult(JsonSerializer.Deserialize<TValue>(value) ?? null);
            }
            catch
            {
                // Ignore and fall back to default
            }
        }

        var defaultValue = GetDefault<TValue>(key);
        return Task.FromResult(defaultValue ?? null);
    }

    public Task Set(string key, object? value)
    {
        var stringValue = GetStringValue(value);

        var defaultConfiguration = configuration.Value.DefaultSettings.GetValueOrDefault(key);
        if (defaultConfiguration != null)
        {
            var defaultValue = GetStringValue(defaultConfiguration);
            if (defaultValue == stringValue)
            {
                stringValue = null;
            }
        }

        var changed = false;
        if (stringValue == null)
        {
            if (store.Remove(key))
            {
                changed = true;
            }
        }
        else
        {
            if (!store.TryGetValue(key, out var existing) || existing != stringValue)
            {
                store[key] = stringValue;
                changed = true;
            }
        }

        if (changed)
        {
            OnSettingsChanged?.Invoke([key]);
        }

        return Task.CompletedTask;
    }

    public Task<bool> IsSettingModified(params string[] keys)
    {
        if (keys.Length == 0)
        {
            return Task.FromResult(false);
        }

        foreach (var key in keys)
        {
            if (!store.TryGetValue(key, out var stored))
            {
                continue;
            }

            var defaultValue = GetStringValue(configuration.Value.DefaultSettings.GetValueOrDefault(key));
            if (defaultValue != stored)
            {
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }

    public Task DeleteSetting(params string[] keys)
    {
        if (keys.Length == 0)
        {
            return Task.CompletedTask;
        }

        var changed = false;
        foreach (var key in keys)
        {
            if (store.Remove(key))
            {
                changed = true;
            }
        }

        if (changed)
        {
            OnSettingsChanged?.Invoke(keys);
        }

        return Task.CompletedTask;
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

    private TValue? GetDefault<TValue>(string key)
    {
        if (configuration.Value.DefaultSettings.TryGetValue(key, out var value) && value is TValue typedValue)
        {
            return typedValue;
        }

        return default;
    }
}

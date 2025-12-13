using System.Globalization;
using System.Text.Json;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.Poe.Tests.Mocks;

public class TestSettingsService : ISettingsService
{
    public event Action<string[]>? OnSettingsChanged;
    private readonly Dictionary<string, string?> store = new();

    public Task<bool> GetBool(string key)
    {
        if (store.TryGetValue(key, out var value) && bool.TryParse(value, out var boolValue))
        {
            return Task.FromResult(boolValue);
        }

        var defaultProperty = typeof(DefaultSettings).GetProperty(key);
        if (defaultProperty == null)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult((bool)(defaultProperty.GetValue(null) ?? false));
    }

    public Task<string?> GetString(string key)
    {
        if (store.TryGetValue(key, out var value))
        {
            return Task.FromResult(value);
        }

        var defaultProperty = typeof(DefaultSettings).GetProperty(key);
        if (defaultProperty == null)
        {
            return Task.FromResult<string?>(null);
        }

        return Task.FromResult((string?)defaultProperty.GetValue(null));
    }

    public Task<int> GetInt(string key)
    {
        if (store.TryGetValue(key, out var value) && int.TryParse(value, out var intValue))
        {
            return Task.FromResult(intValue);
        }

        var defaultProperty = typeof(DefaultSettings).GetProperty(key);
        if (defaultProperty == null)
        {
            return Task.FromResult(0);
        }

        return Task.FromResult((int)(defaultProperty.GetValue(null) ?? 0));
    }

    public Task<DateTimeOffset?> GetDateTime(string key)
    {
        if (store.TryGetValue(key, out var value) && DateTimeOffset.TryParse(value, out var dateValue))
        {
            return Task.FromResult<DateTimeOffset?>(dateValue);
        }

        var defaultProperty = typeof(DefaultSettings).GetProperty(key);
        if (defaultProperty == null)
        {
            return Task.FromResult<DateTimeOffset?>(null);
        }

        return Task.FromResult((DateTimeOffset?)defaultProperty.GetValue(null));
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

        var defaultProperty = typeof(DefaultSettings).GetProperty(key);
        if (defaultProperty == null)
        {
            return Task.FromResult<TEnum?>(null);
        }

        var propertyValue = defaultProperty.GetValue(null)?.ToString();
        if (Enum.TryParse<TEnum>(propertyValue, out var defaultValue))
        {
            return Task.FromResult<TEnum?>(defaultValue);
        }

        var defaultEnumValueFromAttribute = propertyValue?.GetEnumFromValue<TEnum>();
        return Task.FromResult(defaultEnumValueFromAttribute);
    }

    public Task<TValue?> GetObject<TValue>(string key)
    {
        if (store.TryGetValue(key, out var value))
        {
            try
            {
                return Task.FromResult(JsonSerializer.Deserialize<TValue>(value ?? string.Empty));
            }
            catch
            {
                // Ignore and fall back to default
            }
        }

        var defaultProperty = typeof(DefaultSettings).GetProperty(key);
        if (defaultProperty == null)
        {
            return Task.FromResult<TValue?>(default);
        }

        return Task.FromResult((TValue?)(defaultProperty.GetValue(null) ?? null));
    }

    public Task Set(string key, object? value)
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

            var defaultProperty = typeof(DefaultSettings).GetProperty(key);
            if (defaultProperty == null)
            {
                continue;
            }

            var defaultValue = GetStringValue(defaultProperty.GetValue(null));
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
}

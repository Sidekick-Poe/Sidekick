namespace Sidekick.Common.Enums;

/// <summary>
///     Contains extension methods for enums.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    ///     Gets the value associated with the enum.
    /// </summary>
    /// <param name="value">The enum to get the value for.</param>
    /// <param name="key">The key to identify the specific value to return.</param>
    /// <returns>The value associated with the enum.</returns>
    public static string GetValueAttribute(this Enum value, string key = "default")
    {
        var type = value.GetType();
        var memberInfo = type.GetMember(value.ToString());
        if (memberInfo.Length == 0) return value.ToString();

        var attributes = memberInfo[0].GetCustomAttributes(typeof(EnumValueAttribute), false);
        foreach (EnumValueAttribute attribute in attributes)
        {
            if (attribute.Key == key) return attribute.Value;
        }

        return value.ToString();
    }

    /// <summary>
    ///     Gets the enum from an enum value.
    /// </summary>
    /// <typeparam name="T">The type of enum to return.</typeparam>
    /// <param name="value">The value to match with.</param>
    /// <returns>The enum if it is found, or null.</returns>
    public static T? GetEnumFromValue<T>(this string? value)
        where T : Enum
    {
        var type = typeof(T);
        var attributeType = typeof(EnumValueAttribute);
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            var attributes = field.GetCustomAttributes(attributeType, false);
            if (attributes.Length == 0)
            {
                if (field.Name == value)
                {
                    return (T?)field.GetValue(null) ?? default;
                }

                continue;
            }

            foreach (EnumValueAttribute attribute in attributes)
            {
                if (attribute.Value == value && !attribute.IsDuplicate)
                {
                    return (T?)field.GetValue(null) ?? default;
                }
            }
        }

        return default;
    }
}

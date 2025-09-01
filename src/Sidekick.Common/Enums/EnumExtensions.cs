namespace Sidekick.Common.Enums;

/// <summary>
///     Contains extension methods for enums.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    ///     Gets an attribute from an enum.
    /// </summary>
    /// <typeparam name="T">The type of attribute to get.</typeparam>
    /// <param name="value">The value to get the attribute for.</param>
    /// <returns>The attribute if it is found, otherwise returns null.</returns>
    public static T? GetAttribute<T>(this Enum value)
        where T : Attribute
    {
        var type = value.GetType();
        var memberInfo = type.GetMember(value.ToString());
        var attributes = memberInfo[0]
            .GetCustomAttributes(attributeType: typeof(T), false);
        return attributes.Length > 0 ? (T)attributes[0] : null;
    }

    /// <summary>
    ///     Gets the value associated with the enum.
    /// </summary>
    /// <param name="value">The enum to get the value for.</param>
    /// <returns>The value associated with the enum.</returns>
    public static string GetValueAttribute(this Enum value)
    {
        var attribute = value.GetAttribute<EnumValueAttribute>();
        return attribute != null ? attribute.Value : value.ToString();
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

            var attribute = ((EnumValueAttribute)attributes[0]);
            if (attribute.Value == value && !attribute.IsDuplicate)
            {
                return (T?)field.GetValue(null) ?? default;
            }
        }

        return default;
    }
}

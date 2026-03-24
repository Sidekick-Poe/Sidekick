namespace Sidekick.Common.Enums;

/// <summary>
///     Attribute associating a string value to the enum.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="EnumValueAttribute" /> class.
/// </remarks>
/// <param name="value">The string value.</param>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class GameValueAttribute(string value, string key = "default") : Attribute
{
    /// <summary>
    ///     Gets the value associated with the enum.
    /// </summary>
    public string Value { get; } = value;

    /// <summary>
    ///     Gets the key associated with the enum value.
    /// </summary>
    public string Key { get; } = key;
}

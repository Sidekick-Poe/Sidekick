namespace Sidekick.Common.Enums;

/// <summary>
///     Attribute associating a string value to the enum.
/// </summary>
public class EnumValueAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EnumValueAttribute" /> class.
    /// </summary>
    /// <param name="value">The string value.</param>
    public EnumValueAttribute(string? value)
    {
        Value = value;
    }

    /// <summary>
    ///     Gets the value associated with the enum.
    /// </summary>
    public string? Value { get; }
}

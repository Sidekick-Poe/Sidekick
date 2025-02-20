namespace Sidekick.Common.Enums;

/// <summary>
///     Attribute associating a string value to the enum.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="EnumValueAttribute" /> class.
/// </remarks>
/// <param name="value">The string value.</param>
public class EnumValueAttribute(string value) : Attribute
{

    /// <summary>
    ///     Gets the value associated with the enum.
    /// </summary>
    public string Value { get; } = value;
}

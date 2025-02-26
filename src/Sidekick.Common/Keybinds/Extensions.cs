namespace Sidekick.Common.Keybinds;

/// <summary>
///     Extension methods to support settings.
/// </summary>
public static class Extensions
{
    /// <summary>
    ///     Gets a more user readable string value that represents a keybind from the original value.
    /// </summary>
    /// <param name="source">The keybind to get a readable format.</param>
    /// <returns>The readable keybind.</returns>
    public static string? ToKeybindString(this string? source)
    {
        return source
               ?.Replace(" ", "")
               .Replace("+", " + ")
               .Replace(",", ", ");
    }

    public static bool ContainsDuplicates<T>(this IEnumerable<T> enumerable)
    {
        var hashSet = new HashSet<T>();
        return !enumerable.All(hashSet.Add);
    }
}

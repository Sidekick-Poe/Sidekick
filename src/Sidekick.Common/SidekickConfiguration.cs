using System.Reflection;

namespace Sidekick.Common;

/// <summary>
///     Configuration class for the application.
/// </summary>
public static class SidekickConfiguration
{
    /// <summary>
    ///     Gets or sets a list of modules.
    /// </summary>
    public static List<Assembly> Modules { get; } = [];

    /// <summary>
    ///     The list of keybinds handled by this application
    /// </summary>
    public static List<Type> Keybinds { get; } = [];
}

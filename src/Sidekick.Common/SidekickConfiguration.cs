using System.Reflection;
using Sidekick.Common.Settings;

namespace Sidekick.Common;

/// <summary>
///     Configuration class for the application.
/// </summary>
public class SidekickConfiguration
{
    /// <summary>
    ///     Gets or sets a list of initializable services.
    /// </summary>
    public List<Type> InitializableServices { get; } = new();

    /// <summary>
    ///     Gets or sets a list of modules.
    /// </summary>
    public List<Assembly> Modules { get; } = new();

    /// <summary>
    ///     The list of keybinds handled by this application
    /// </summary>
    public List<Type> Keybinds { get; } = new();
}

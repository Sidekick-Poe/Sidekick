using System.Reflection;

namespace Sidekick.Common;

/// <summary>
///     Configuration class for the application.
/// </summary>
public static class SidekickConfiguration
{
    private static bool isXselPackageMissing;

    /// <summary>
    /// Occurs when a relevant flag or state within the application configuration changes.
    /// </summary>
    public static event Action? FlagChanged;

    /// <summary>
    /// Gets or sets a value indicating whether the xsel package is missing.
    /// Linux only.
    /// </summary>
    public static bool IsXselPackageMissing
    {
        get => isXselPackageMissing;
        set
        {
            isXselPackageMissing = value;
            FlagChanged?.Invoke();
        }
    }

    /// <summary>
    ///     Gets or sets a list of initializable services.
    /// </summary>
    public static List<Type> InitializableServices { get; } = new();

    /// <summary>
    ///     Gets or sets a list of modules.
    /// </summary>
    public static List<Assembly> Modules { get; } = new();

    /// <summary>
    ///     The list of input handlers handled by this application
    /// </summary>
    public static List<Type> InputHandlers { get; } = new();
}

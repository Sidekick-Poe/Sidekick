using System.Reflection;

namespace Sidekick.Common;

/// <summary>
///     Configuration class for the application.
/// </summary>
public class SidekickConfiguration
{
    private bool isXselPackageMissing;

    /// <summary>
    /// Occurs when a relevant flag or state within the application configuration changes.
    /// </summary>
    public event Action? FlagChanged;

    /// <summary>
    /// Gets or sets a value indicating whether the xsel package is missing.
    /// Linux only.
    /// </summary>
    public bool IsXselPackageMissing
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
    public List<Type> InitializableServices { get; } = new();

    /// <summary>
    ///     Gets or sets a list of modules.
    /// </summary>
    public List<Assembly> Modules { get; } = new();

    /// <summary>
    ///     The list of input handlers handled by this application
    /// </summary>
    public List<Type> InputHandlers { get; } = new();

    public Dictionary<string, object> DefaultSettings { get; } = new();

    public SidekickApplicationType ApplicationType { get; set; }

    public bool SupportsKeybinds => ApplicationType == SidekickApplicationType.Wpf
                                    || ApplicationType == SidekickApplicationType.Photino;

    public bool SupportsAuthentication => ApplicationType == SidekickApplicationType.Wpf;

    public bool SupportsHardwareAcceleration => ApplicationType == SidekickApplicationType.Wpf;
}
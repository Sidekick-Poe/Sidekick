using Sidekick.Common.Initialization;

namespace Sidekick.Common.Platform;

/// <summary>
///     Interface containing platform specific methods.
/// </summary>
public interface IApplicationService : IInitializableService
{
    /// <summary>
    /// Retrieves the current version of the application.
    /// </summary>
    /// <returns>
    /// A string representing the version of the application, or null if the version is unavailable.
    /// </returns>
    string? GetVersion()
    {
        List<string?> assemblies =
        [
            "Sidekick.Wpf",
            "Sidekick.Avalonia",
            "Sidekick.Web",
        ];
        var version = AppDomain.CurrentDomain.GetAssemblies().Select(x => x.GetName()).FirstOrDefault(x => assemblies.Contains(x.Name))?.Version;
        return version?.ToString();
    }

    /// <summary>
    ///     Shutdown the application.
    /// </summary>
    void Shutdown();

    /// <summary>
    /// Gets or sets a value indicating whether the application has initialized.
    /// </summary>
    bool HasInitialized { get; set; }
}

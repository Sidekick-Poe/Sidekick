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
        var version = AppDomain.CurrentDomain.GetAssemblies().Select(x => x.GetName()).FirstOrDefault(x => x.Name == "Sidekick")?.Version;
        return version?.ToString();
    }

    /// <summary>
    ///     Shutdown the application.
    /// </summary>
    void Shutdown();
}

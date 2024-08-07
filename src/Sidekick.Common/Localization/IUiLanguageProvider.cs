using System.Globalization;
using Sidekick.Common.Initialization;

namespace Sidekick.Common.Localization;

/// <summary>
///     Interface to access ui language parameters.
/// </summary>
public interface IUiLanguageProvider : IInitializableService
{
    /// <summary>
    ///     Gets the list of available UI languages
    /// </summary>
    List<CultureInfo> GetList();

    /// <summary>
    ///     Sets the Ui language
    /// </summary>
    /// <param name="name">The culture name of the desired language</param>
    void Set(string? name);
}

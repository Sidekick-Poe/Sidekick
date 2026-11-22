using Microsoft.Extensions.Localization;
using Sidekick.Common.Platform.Localization;

namespace Sidekick.Common.Platform.Linux.Dependencies;

/// <summary>
/// A Linux package that Sidekick may need. Each implementation defines how to check for the package and its names per distribution.
/// </summary>
public interface IDependency
{
	/// <summary>
	/// The text that names the package and says what it is used for.
	/// </summary>
	Func<IStringLocalizer<PlatformResources>, string> Description { get; }

	/// <summary>
	/// Returns true when the dependency is satisfied on the current system.
	/// </summary>
	bool IsAvailable();

	string ArchPackage { get; }
	string DebianPackage { get; }
	string FedoraPackage { get; }
}

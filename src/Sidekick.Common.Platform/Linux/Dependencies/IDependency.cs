namespace Sidekick.Common.Platform.Linux.Dependencies;

/// <summary>
/// A Linux package that Sidekick may need. Each implementation defines how to check for the package and its names per distribution.
/// </summary>
public interface IDependency
{
	/// <summary>
	/// The text that names the package and says what it is used for.
	/// </summary>
	string Description { get; }

	/// <summary>
	/// Returns true when the dependency is satisfied on the current system.
	/// </summary>
	bool IsAvailable();

	/// <summary>
	/// The package name on Arch Linux.
	/// </summary>
	string ArchPackage { get; }

	/// <summary>
	/// The package name on Debian and Ubuntu.
	/// </summary>
	string DebianPackage { get; }

	/// <summary>
	/// The package name on Fedora.
	/// </summary>
	string FedoraPackage { get; }
}

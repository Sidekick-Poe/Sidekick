namespace Sidekick.Common.Platform.Linux;

using Sidekick.Common.Platform.Linux.Dependencies;

public static class LinuxDependencyChecker
{
	private static readonly IDependency[] Dependencies =
	[
		new XselDependency(),
		new WebKitGtkDependency(),
	];

	public static IEnumerable<IDependency> GetMissingDependencies()
	{
		if (!OperatingSystem.IsLinux())
		{
			yield break;
		}

		foreach (var dependency in Dependencies)
		{
			if (!dependency.IsAvailable())
			{
				yield return dependency;
			}
		}
	}

	/// <summary>
	/// Gets the command for the current distribution, or the commands for every supported distribution when it is not recognized.
	/// </summary>
	public static IReadOnlyList<LinuxInstallCommand> GetInstallCommands(IReadOnlyList<IDependency> missingDependencies)
	{
		var arch = new LinuxInstallCommand("Arch", $"sudo pacman -S --needed {string.Join(' ', missingDependencies.Select(x => x.ArchPackage))}");
		var debian = new LinuxInstallCommand("Ubuntu / Debian", $"sudo apt install {string.Join(' ', missingDependencies.Select(x => x.DebianPackage))}");
		var fedora = new LinuxInstallCommand("Fedora", $"sudo dnf install {string.Join(' ', missingDependencies.Select(x => x.FedoraPackage))}");

		// ID_LIKE lists the distributions a derivative is based on, like "ubuntu debian" on Linux Mint.
		var ids = GetDistributionIds();
		if (ids.Contains("arch")) return [arch];
		if (ids.Contains("debian") || ids.Contains("ubuntu")) return [debian];
		if (ids.Contains("fedora")) return [fedora];

		return [arch, debian, fedora];
	}

	private static HashSet<string> GetDistributionIds()
	{
		var ids = new HashSet<string>();
		try
		{
			foreach (var line in File.ReadLines("/etc/os-release"))
			{
				if (!line.StartsWith("ID=") && !line.StartsWith("ID_LIKE=")) continue;

				var value = line[(line.IndexOf('=') + 1)..].Trim('"', '\'');
				ids.UnionWith(value.Split(' ', StringSplitOptions.RemoveEmptyEntries));
			}
		}
		catch (IOException)
		{
		}
		catch (UnauthorizedAccessException)
		{
		}

		return ids;
	}
}

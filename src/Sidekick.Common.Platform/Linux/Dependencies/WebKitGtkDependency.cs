using System.Runtime.InteropServices;
using Microsoft.Extensions.Localization;
using Sidekick.Common.Platform.Localization;

namespace Sidekick.Common.Platform.Linux.Dependencies;

/// <summary>
/// Shows the content of every Sidekick window. The windows are web pages (Blazor), and on Linux Avalonia's web view
/// uses the system's WebKitGTK instead of shipping its own browser engine.
/// </summary>
internal sealed class WebKitGtkDependency : IDependency
{
	public Func<IStringLocalizer<PlatformResources>, string> Description => resources => resources.Linux_Dependencies_WebKitGtk;

	public bool IsAvailable()
	{
		// 1. Strict native loader check using standard SONAME variants across modern and legacy distributions
		string[] exactWebKitLibs =
		[
			"libwebkit2gtk-4.1.so.0", // Modern standard (Ubuntu 22.04+, Fedora, Arch)
			"libwebkitgtk-6.0.so.4",   // GTK4 modern variant
			"libwebkit2gtk-4.0.so.37", // Legacy fallback (Ubuntu 20.04 / Debian Oldstable)
			"libwebkit2gtk-4.1.so",
			"libwebkit2gtk-4.0.so"
		];

		foreach (var libName in exactWebKitLibs)
		{
			if (NativeLibrary.TryLoad(libName, out var handle))
			{
				NativeLibrary.Free(handle);
				return true;
			}
		}

		// 2. Local filesystem check for common dynamic linker absolute paths
		string[] knownLoaderPaths =
		[
			// WebKit2GTK 4.1
			"/usr/lib/x86_64-linux-gnu/libwebkit2gtk-4.1.so.0",
			"/usr/lib/libwebkit2gtk-4.1.so.0",
			"/usr/lib64/libwebkit2gtk-4.1.so.0",

			// WebKitGTK 6.0
			"/usr/lib/x86_64-linux-gnu/libwebkitgtk-6.0.so.4",
			"/usr/lib/libwebkitgtk-6.0.so.4",

			// Legacy WebKit2GTK 4.0
			"/usr/lib/x86_64-linux-gnu/libwebkit2gtk-4.0.so.37",
			"/usr/lib/libwebkit2gtk-4.0.so.37",
			"/usr/lib64/libwebkit2gtk-4.0.so.37"
		];

		foreach (var path in knownLoaderPaths)
		{
			if (File.Exists(path))
			{
				return true;
			}
		}

		return false;
	}

	public string ArchPackage => "webkit2gtk-4.1";
	public string DebianPackage => "libwebkit2gtk-4.1-0";
	public string FedoraPackage => "webkit2gtk4.1";
}

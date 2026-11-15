using System.Runtime.InteropServices;

namespace Sidekick.Avalonia.Utilities;

/// <summary>
/// A package Sidekick needs on Linux, with its name in each supported distribution.
/// </summary>
/// <param name="ResourceKey">The key of the text that names the package and says what it is used for, in UiResources.</param>
public record LinuxDependency(string ResourceKey, string ArchPackage, string DebianPackage, string FedoraPackage);

/// <summary>
/// The command that installs the missing packages on a family of distributions.
/// </summary>
public record LinuxInstallCommand(string Distribution, string Command);

public static class LinuxDependencyChecker
{
    private static readonly LinuxDependency Xsel = new(
        ResourceKey: "Linux_Dependencies_Xsel",
        ArchPackage: "xsel",
        DebianPackage: "xsel",
        FedoraPackage: "xsel");

    private static readonly LinuxDependency WebKitGtk = new(
        ResourceKey: "Linux_Dependencies_WebKitGtk",
        ArchPackage: "webkit2gtk-4.1",
        DebianPackage: "libwebkit2gtk-4.1-0",
        FedoraPackage: "webkit2gtk4.1");

    public static IEnumerable<LinuxDependency> GetMissingDependencies()
    {
        if (!OperatingSystem.IsLinux())
        {
            yield break;
        }

        if (!IsCommandAvailable("xsel"))
        {
            yield return Xsel;
        }

        if (!IsWebKitGtkAvailable())
        {
            yield return WebKitGtk;
        }
    }

    /// <summary>
    /// Gets the command for the current distribution, or the commands for every supported distribution when it is not recognized.
    /// </summary>
    public static IReadOnlyList<LinuxInstallCommand> GetInstallCommands(IReadOnlyList<LinuxDependency> missingDependencies)
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

    private static bool IsCommandAvailable(string command)
    {
        var pathEnv = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrEmpty(pathEnv))
        {
            return false;
        }

        var paths = pathEnv.Split(':');
        foreach (var path in paths)
        {
            var fullPath = Path.Combine(path, command);
            if (File.Exists(fullPath))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsWebKitGtkAvailable()
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
}

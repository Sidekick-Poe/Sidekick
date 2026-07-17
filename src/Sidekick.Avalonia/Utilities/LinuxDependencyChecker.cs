using System.Runtime.InteropServices;

namespace Sidekick.Avalonia.Utilities;

public static class LinuxDependencyChecker
{
    public static IEnumerable<string> GetMissingDependencies()
    {
        if (!OperatingSystem.IsLinux())
        {
            yield break;
        }

        if (!IsCommandAvailable("xsel"))
        {
            yield return "xsel";
        }

        if (!IsWebKitGtkAvailable())
        {
            yield return "WebKitGTK runtime (libwebkit2gtk)";
        }
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
    
    public static string BuildDialogMessage(IReadOnlyList<string> missingDependencies)
    {
        var items = string.Join(Environment.NewLine, missingDependencies.Select(d => $"- {d}"));

        return $"""
            Missing Linux dependencies. Sidekick requires the following packages on Linux:
            
            {items}
            
            Install them using one of the following commands:
            
            Arch-based:
            sudo pacman -S --needed xsel webkit2gtk-4.1
            
            Ubuntu / Debian:
            sudo apt install xsel libwebkit2gtk-4.1-0
            
            Fedora:
            sudo dnf install xsel webkit2gtk4.1
            """;
    }
}
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
namespace Sidekick.Avalonia.Utilities;

[SupportedOSPlatform("linux")]
internal static class LinuxNativeMethods
{
    /// <summary>
    ///     Sets an environment variable in the native process environment, which native libraries such as WebKitGTK read.
    ///     Environment.SetEnvironmentVariable only changes the copy kept by .NET on Linux.
    /// </summary>
    /// <param name="name">The variable name.</param>
    /// <param name="value">The variable value.</param>
    /// <param name="overwrite">0 keeps a value that is already set, any other value replaces it.</param>
    [DllImport("libc", SetLastError = true)]
    internal static extern int setenv(string name, string value, int overwrite);
}

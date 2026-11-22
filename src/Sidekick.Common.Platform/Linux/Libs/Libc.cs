using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Sidekick.Common.Platform.Linux.X11;

/// <summary>
/// P/Invoke wrappers for libc functions used by the X11 event loop and platform initialization.
/// These are not part of libX11 but are needed for non-blocking event polling and environment setup.
/// </summary>
[SupportedOSPlatform(PlatformNames.Linux)]
internal static class Libc
{
    private const string LibraryName = "libc";

    /// <summary>
    /// Sets an environment variable in the native process environment, which native libraries such as WebKitGTK read.
    /// <c>Environment.SetEnvironmentVariable</c> only changes the copy kept by .NET on Linux.
    /// </summary>
    /// <param name="name">The variable name.</param>
    /// <param name="value">The variable value.</param>
    /// <param name="overwrite">0 keeps a value that is already set, any other value replaces it.</param>
    [DllImport(LibraryName, SetLastError = true)]
    public static extern int setenv(string name, string value, int overwrite);

    /// <summary>
    /// Waits for I/O events on one or more file descriptors, blocking until at least one fd is ready or the timeout expires.
    /// Used to wait for X11 events without busy-spinning.
    /// </summary>
    [DllImport(LibraryName, SetLastError = true)]
    public static extern int poll([In, Out] PollFd[] fds, nuint nfds, int timeout);

    [DllImport(LibraryName, SetLastError = true)]
    public static extern int pipe([Out] int[] fds);

    [DllImport(LibraryName, SetLastError = true)]
    public static extern nint read(int fd, byte[] buffer, nint count);

    [DllImport(LibraryName, SetLastError = true)]
    public static extern nint write(int fd, byte[] buffer, nint count);

    [DllImport(LibraryName, SetLastError = true)]
    public static extern int close(int fd);

    public const short POLLIN = 1;

    [StructLayout(LayoutKind.Sequential)]
    public struct PollFd
    {
        public int Fd;
        public short Events;
        public short Revents;
    }
}

namespace Sidekick.Common.Platform;

/// <summary>
/// Detects a Wayland session on Linux and its desktop. KDE does not sync copies made in native Wayland windows (like a
/// game running in Proton's Wayland mode) to the X11 clipboard that xsel reads, so Sidekick reads the Wayland clipboard
/// there. GNOME syncs them, and its wl-paste has to open a window to read, so GNOME keeps xsel.
/// </summary>
public static class WaylandSession
{
    /// <summary>
    /// Gets the Wayland display saved by <see cref="Initialize"/>, or null outside a Wayland session.
    /// </summary>
    public static string? Display { get; private set; }

    /// <summary>
    /// Gets a value indicating whether Sidekick runs in a Wayland session, even when Sidekick itself draws its windows
    /// through XWayland.
    /// </summary>
    public static bool IsActive => Display != null;

    /// <summary>
    /// Gets a value indicating whether the Wayland session runs the KDE Plasma desktop.
    /// </summary>
    public static bool IsKde { get; private set; }

    /// <summary>
    /// Saves WAYLAND_DISPLAY and the desktop. Must run before Avalonia starts: its X11 backend replaces WAYLAND_DISPLAY
    /// with a fake display so GTK does not use Wayland, and wl-clipboard inherits that fake value.
    /// </summary>
    public static void Initialize()
    {
        var display = Environment.GetEnvironmentVariable("WAYLAND_DISPLAY");
        Display = OperatingSystem.IsLinux() && !string.IsNullOrEmpty(display) ? display : null;

        // XDG_CURRENT_DESKTOP can list several names, separated by colons.
        var desktops = Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP")?.Split(':') ?? [];
        IsKde = IsActive && desktops.Contains("KDE", StringComparer.OrdinalIgnoreCase);
    }
}

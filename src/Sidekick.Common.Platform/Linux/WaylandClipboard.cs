using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Platform.Clipboard;

namespace Sidekick.Common.Platform.Linux;

/// <summary>
/// The clipboard on KDE Wayland, through wl-clipboard. KDE does not sync a copy made in a native Wayland window (like a
/// game in Proton's Wayland mode) to the X11 clipboard that xsel reads. Copies made in X11 windows do reach the Wayland
/// clipboard, so this works for games running through XWayland too.
/// </summary>
public class WaylandClipboard(ILogger<WaylandClipboard> logger) : ISystemClipboard
{
    /// <summary>
    /// Gets a value indicating whether wl-clipboard is installed. It is optional: without it, Sidekick uses xsel, which
    /// works for every game except one running as a native Wayland window on KDE.
    /// </summary>
    public static bool IsInstalled => LinuxCommand.IsAvailable("wl-paste") && LinuxCommand.IsAvailable("wl-copy");

    /// <summary>
    /// How long to wait for wl-paste or wl-copy before giving up, so a stuck command can't block a price check.
    /// Both answer within a few milliseconds.
    /// </summary>
    private static readonly TimeSpan CommandTimeout = TimeSpan.FromMilliseconds(500);

    /// <inheritdoc/>
    public async Task<string?> GetText()
    {
        try
        {
            // --type text skips non-text content, like images. wl-paste fails when the clipboard is empty.
            var startInfo = CreateStartInfo("wl-paste", ["--no-newline", "--type", "text"]);
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                return null;
            }

            using var timeout = new CancellationTokenSource(CommandTimeout);
            var output = process.StandardOutput.ReadToEndAsync(timeout.Token);
            var error = process.StandardError.ReadToEndAsync(timeout.Token);
            await WaitForExit(process, timeout.Token);

            if (process.ExitCode != 0)
            {
                logger.LogDebug("[Clipboard] wl-paste returned no text: {Error}", (await error).Trim());
                return null;
            }

            return await output;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("[Clipboard] wl-paste did not answer within {Timeout} ms.", CommandTimeout.TotalMilliseconds);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Clipboard] Failed to get the text value from the clipboard with wl-paste.");
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task SetText(string? text)
    {
        try
        {
            // wl-copy keeps a background process that serves the text until the clipboard changes. Its output is not
            // redirected, since that process would keep the pipes open and the wait below would never end.
            var startInfo = CreateStartInfo("wl-copy", ["--type", "text/plain;charset=utf-8"]);
            startInfo.RedirectStandardInput = true;

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                return;
            }

            await process.StandardInput.WriteAsync(text ?? string.Empty);
            process.StandardInput.Close();

            using var timeout = new CancellationTokenSource(CommandTimeout);
            await WaitForExit(process, timeout.Token);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("[Clipboard] wl-copy did not answer within {Timeout} ms.", CommandTimeout.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Clipboard] Failed to set the text value to the clipboard with wl-copy.");
        }
    }

    /// <summary>
    /// Waits for the command to finish, and kills it when it doesn't answer in time, so a hung command doesn't stay
    /// behind after every clipboard read.
    /// </summary>
    private static async Task WaitForExit(Process process, CancellationToken cancellationToken)
    {
        try
        {
            await process.WaitForExitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            process.Kill();
            throw;
        }
    }

    private static ProcessStartInfo CreateStartInfo(string command, string[] arguments)
    {
        var startInfo = new ProcessStartInfo(command, arguments)
        {
            UseShellExecute = false,
        };

        // Avalonia replaces WAYLAND_DISPLAY with a fake value, see WaylandSession.
        startInfo.Environment["WAYLAND_DISPLAY"] = WaylandSession.Display;

        return startInfo;
    }
}


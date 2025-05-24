using System.Diagnostics;

namespace Sidekick.Common.Platform.Linux;

public static class PackageVerifier
{
    /// <summary>
    /// Checks if the xsel package is installed on the system.
    /// Necessary for TextCopy.
    /// </summary>
    public static bool IsXselInstalled()
    {
        try
        {
            // Create a process to run the "which xsel" command
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "which",
                    Arguments = "xsel",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();

            // If the exit code is 0, xsel is installed
            return process.ExitCode == 0;
        }
        catch
        {
            // If an exception occurs, assume xsel is not installed
            return false;
        }
    }
}

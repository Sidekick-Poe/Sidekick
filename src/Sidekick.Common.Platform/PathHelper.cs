namespace Sidekick.Common.Platform;

/// <summary>
/// Helpers for searching the PATH environment variable to find installed command-line tools.
/// Works on both Windows (semicolon separator) and Linux (colon separator).
/// </summary>
public static class PathHelper
{
    /// <summary>
    /// Gets a value indicating whether the command is installed in one of the folders of the PATH.
    /// </summary>
    public static bool IsAvailable(string command)
    {
        var pathEnv = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrEmpty(pathEnv))
        {
            return false;
        }

        foreach (var path in pathEnv.Split(Path.PathSeparator))
        {
            if (File.Exists(Path.Combine(path, command)))
            {
                return true;
            }
        }

        return false;
    }
}

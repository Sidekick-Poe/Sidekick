using System.Runtime.CompilerServices;

namespace Sidekick.Apis.Poe.Tests;

public static class ResourceHelper
{
    public static string ReadFileContent(string filename, [CallerFilePath] string filePath = "")
    {
        if (filePath is "")
            throw new ArgumentException("filepath missing", nameof(filePath));

        var path = Path.Combine(new FileInfo(filePath).Directory!.FullName, filename);
        return File.ReadAllText(path);
    }
}

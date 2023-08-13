namespace Sidekick.Common
{
    public static class SidekickPaths
    {
        public static string GetDataFilePath(string path = "")
        {
            var environmentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var sidekickFolder = Path.Combine(environmentFolder, "sidekick");

            if (!Directory.Exists(sidekickFolder))
            {
                Directory.CreateDirectory(sidekickFolder);
            }

            if (!string.IsNullOrEmpty(path))
            {
                return Path.Combine(sidekickFolder, path);
            }

            return sidekickFolder;
        }
    }
}

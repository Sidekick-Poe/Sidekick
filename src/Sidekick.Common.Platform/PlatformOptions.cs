using System.Runtime.InteropServices;

namespace Sidekick.Common.Platform
{
    /// <summary>
    /// Options to configure the platform
    /// </summary>
    public class PlatformOptions
    {
        /// <summary>
        /// The path to the icon for use on Windows machines. The file should be a BMP/ICO
        /// </summary>
        public string? WindowsIconPath { get; set; }

        /// <summary>
        /// The path to the icon for use on Osx machines. The file should be a PNG/JPG
        /// </summary>
        public string? OsxIconPath { get; set; }

        /// <summary>
        /// The path to the icon. Needs both WindowsIconPath and OsxIconPath properties to be filled
        /// to work properly.
        /// </summary>
        public string? IconPath
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return WindowsIconPath;
                }

                return OsxIconPath;
            }
        }
    }
}

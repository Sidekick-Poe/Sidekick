#pragma warning disable CA1806 // Do not ignore method results
#pragma warning disable CA1416 // Validate platform compatibility

using System.Diagnostics;
using System.Security.Principal;
using System.Text;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Platform.Windows.DllImport;
using Sidekick.Common.Platforms.Localization;

namespace Sidekick.Common.Platform.Windows.Processes
{
    public class ProcessProvider : IProcessProvider, IDisposable
    {
        private const string PATH_OF_EXILE_TITLE = "Path of Exile";
        private const string SIDEKICK_TITLE = "Sidekick";
        private static readonly List<string> PossibleProcessNames = new() { "PathOfExile", "PathOfExile_x64", "PathOfExileSteam", "PathOfExile_x64Steam" };

        public string? ClientLogPath
        {
            get
            {
                var directory = Path.GetDirectoryName(GetPathOfExileProcess()?.GetMainModuleFileName());
                if (directory == null)
                {
                    return null;
                }

                return Path.Combine(directory, "logs", "Client.txt");
            }
        }

        private const int STANDARD_RIGHTS_REQUIRED = 0xF0000;
        private const int TOKEN_ASSIGN_PRIMARY = 0x1;
        private const int TOKEN_DUPLICATE = 0x2;
        private const int TOKEN_IMPERSONATE = 0x4;
        private const int TOKEN_QUERY = 0x8;
        private const int TOKEN_QUERY_SOURCE = 0x10;
        private const int TOKEN_ADJUST_GROUPS = 0x40;
        private const int TOKEN_ADJUST_PRIVILEGES = 0x20;
        private const int TOKEN_ADJUST_SESSIONID = 0x100;
        private const int TOKEN_ADJUST_DEFAULT = 0x80;
        private const int TOKEN_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY | TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE | TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_SESSIONID | TOKEN_ADJUST_DEFAULT;

        private readonly ILogger logger;
        private readonly IApplicationService applicationService;
        private readonly PlatformResources platformResources;

        private bool PermissionChecked { get; set; } = false;
        private bool HasInitialized { get; set; } = false;
        private CancellationTokenSource? WindowsHook { get; set; }

        private string FocusedWindow
        {
            get
            {
                // Create the variable
                const int nChar = 256;
                var ss = new StringBuilder(nChar);

                // Run GetForeGroundWindows and get active window informations
                // assign them into handle pointer variable
                if (User32.GetWindowText(User32.GetForegroundWindow(), ss, nChar) > 0)
                {
                    return ss.ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        /// <inheritdoc/>
        public bool IsPathOfExileInFocus => FocusedWindow == PATH_OF_EXILE_TITLE;

        /// <inheritdoc/>
        public bool IsSidekickInFocus => FocusedWindow == SIDEKICK_TITLE;

        public ProcessProvider(
            ILogger<ProcessProvider> logger,
            IApplicationService applicationService,
            PlatformResources platformResources)
        {
            this.logger = logger;
            this.applicationService = applicationService;
            this.platformResources = platformResources;
        }

        public void Initialize()
        {
            // We can't initialize twice
            if (HasInitialized)
            {
                return;
            }

            WindowsHook = EventLoop.Run(WinEvent.EVENT_SYSTEM_FOREGROUND, WinEvent.EVENT_SYSTEM_CAPTURESTART, IntPtr.Zero, OnWindowsEvent, 0, 0, WinEvent.WINEVENT_OUTOFCONTEXT);
            HasInitialized = true;
        }

        private void OnWindowsEvent(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (eventType == WinEvent.EVENT_SYSTEM_MINIMIZEEND || eventType == WinEvent.EVENT_SYSTEM_FOREGROUND)
            {
                // If the game is run as administrator, Sidekick also needs administrator privileges.
                if (!PermissionChecked && IsPathOfExileInFocus)
                {
                    PermissionChecked = true;
                    Task.Run(async () =>
                    {
                        if (!IsUserRunAsAdmin() && IsPathOfExileRunAsAdmin())
                        {
                            await RestartAsAdmin();
                        }
                    });
                }
            }
        }

        private static Process? GetPathOfExileProcess()
        {
            foreach (var processName in PossibleProcessNames)
            {
                var process = Process.GetProcessesByName(processName).FirstOrDefault();
                if (process != null)
                {
                    return process;
                }
            }

            return null;
        }

        private async Task RestartAsAdmin()
        {
            if (!await applicationService.OpenConfirmationModal(platformResources.RestartAsAdminText))
            {
                return;
            }

            try
            {
                using var p = new Process();
                p.StartInfo.FileName = "Sidekick.exe";
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.Verb = "runas";
                p.Start();
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "This application must be run as administrator.");
            }
            finally
            {
                applicationService.Shutdown();
            }
        }

        private static bool IsUserRunAsAdmin()
        {
            var info = WindowsIdentity.GetCurrent();
            var principle = new WindowsPrincipal(info);
            return principle.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private bool IsPathOfExileRunAsAdmin()
        {
            var result = false;

            try
            {
                User32.GetWindowThreadProcessId(User32.GetForegroundWindow(), out var processID);
                var proc = Process.GetProcessById(processID);

                User32.OpenProcessToken(proc.Handle, TOKEN_ALL_ACCESS, out var ph);
                using (var iden = new WindowsIdentity(ph))
                {
                    if (iden.Groups != null)
                    {
                        foreach (var role in iden.Groups)
                        {
                            if (role.IsValidTargetType(typeof(SecurityIdentifier)))
                            {
                                var sid = role as SecurityIdentifier;
                                if (sid == null)
                                {
                                    continue;
                                }

                                if (sid.IsWellKnown(WellKnownSidType.AccountAdministratorSid) || sid.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid))
                                {
                                    result = true;
                                    break;
                                }
                            }
                        }
                    }

                    User32.CloseHandle(ph);
                }

                return result;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, e.Message);
            }

            return result;
        }

        public void Dispose()
        {
            WindowsHook?.Cancel();
        }
    }
}

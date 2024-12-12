#pragma warning disable CA1806 // Do not ignore method results
#pragma warning disable CA1416 // Validate platform compatibility

using System.Diagnostics;
using System.Security.Principal;
using System.Text;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Platform.Localization;
using Sidekick.Common.Platform.Windows.DllImport;

namespace Sidekick.Common.Platform.Windows.Processes
{
    public class ProcessProvider
    (
        ILogger<ProcessProvider> logger,
        IApplicationService applicationService,
        ISidekickDialogs dialogService,
        PlatformResources platformResources
    ) : IProcessProvider, IDisposable
    {
        private const string PATH_OF_EXILE_TITLE = "Path of Exile";
        private const string PATH_OF_EXILE_2_TITLE = "Path of Exile 2";
        private const string SIDEKICK_TITLE = "Sidekick";

        private static readonly List<string> PossibleProcessNames = new()
        {
            "PathOfExile",
            "PathOfExile_x64",
            "PathOfExile_KG",
            "PathOfExile_x64_KG",
            "PathOfExileSteam",
            "PathOfExile_x64Steam"
        };

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

        private readonly ILogger logger = logger;

        private bool PermissionChecked { get; set; }

        private bool HasInitialized { get; set; }

        private CancellationTokenSource? WindowsHook { get; set; }

        private DateTimeOffset PreviousFocusedWindowAttempt { get; set; }

        private string? PreviousFocusedWindow { get; set; }

        private string? GetFocusedWindow()
        {
            if (DateTimeOffset.Now - PreviousFocusedWindowAttempt < TimeSpan.FromSeconds(3))
            {
                return PreviousFocusedWindow;
            }

            try
            {
                // Create the variable
                const int NChar = 256;
                var ss = new StringBuilder(NChar);

                // Run GetForeGroundWindows and get active window information
                // assign them into handle pointer variable
                var hWnd = User32.GetForegroundWindow();
                if (User32.GetWindowText(hWnd, ss, NChar) > 0)
                {
                    PreviousFocusedWindow = ss.ToString();
                    // logger.LogDebug("[ProcessProvider] Current focused window title: {0}", PreviousFocusedWindow);
                }
                else
                {
                    // Try to get the process name as a fallback
                    User32.GetWindowThreadProcessId(hWnd, out var processId);
                    var process = Process.GetProcessById(processId);
                    PreviousFocusedWindow = process.ProcessName;
                    // logger.LogDebug("[ProcessProvider] Fallback to process name: {0}", PreviousFocusedWindow);
                }
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "[ProcessProvider] Failed to grab the focused window: {0}", e.Message);
                PreviousFocusedWindow = null;
            }

            return PreviousFocusedWindow;
        }

        /// <inheritdoc/>
        public bool IsPathOfExileInFocus
        {
            get
            {
                var focusedWindow = GetFocusedWindow();
                return focusedWindow is PATH_OF_EXILE_TITLE or PATH_OF_EXILE_2_TITLE;
            }
        }

        /// <inheritdoc/>
        public bool IsSidekickInFocus => GetFocusedWindow()?.StartsWith(SIDEKICK_TITLE) ?? false;

        /// <inheritdoc/>
        public int Priority => 0;

        /// <inheritdoc/>
        public Task Initialize()
        {
            // We can't initialize twice
            if (HasInitialized)
            {
                return Task.CompletedTask;
            }

            WindowsHook = EventLoop.Run(WinEvent.EVENT_SYSTEM_FOREGROUND,
                                        WinEvent.EVENT_SYSTEM_CAPTURESTART,
                                        IntPtr.Zero,
                                        OnWindowsEvent,
                                        0,
                                        0,
                                        WinEvent.WINEVENT_OUTOFCONTEXT);
            HasInitialized = true;

            return Task.CompletedTask;
        }

        private void OnWindowsEvent(
            IntPtr hWinEventHook,
            uint eventType,
            IntPtr hwnd,
            int idObject,
            int idChild,
            uint dwEventThread,
            uint dwmsEventTime)
        {
            if (eventType != WinEvent.EVENT_SYSTEM_MINIMIZEEND && eventType != WinEvent.EVENT_SYSTEM_FOREGROUND)
            {
                return;
            }

            // If the game is run as administrator, Sidekick also needs administrator privileges.
            if (PermissionChecked || !IsPathOfExileInFocus)
            {
                return;
            }

            Task.Run(async () =>
            {
                if (!IsUserRunAsAdmin() && IsPathOfExileRunAsAdmin())
                {
                    await RestartAsAdmin();
                }
            });

            // Once permission has been checked, we can stop this hook from running.
            PermissionChecked = true;
            WindowsHook?.Cancel();
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
            if (!await dialogService.OpenConfirmationModal(platformResources.RestartAsAdminText))
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
            var ph = IntPtr.Zero;

            try
            {
                User32.GetWindowThreadProcessId(User32.GetForegroundWindow(), out var processId);
                var proc = Process.GetProcessById(processId);

                User32.OpenProcessToken(proc.Handle, TOKEN_ALL_ACCESS, out ph);
                using var windowsIdentity = new WindowsIdentity(ph);
                if (windowsIdentity.Groups != null)
                {
                    foreach (var role in windowsIdentity.Groups)
                    {
                        if (!role.IsValidTargetType(typeof(SecurityIdentifier)))
                        {
                            continue;
                        }

                        var sid = role as SecurityIdentifier;
                        if (sid == null)
                        {
                            continue;
                        }

                        if (!sid.IsWellKnown(WellKnownSidType.AccountAdministratorSid) && !sid.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid))
                        {
                            continue;
                        }

                        result = true;
                        break;
                    }
                }

                User32.CloseHandle(ph);

                return result;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, e.Message);
                logger.LogWarning(e, "[ProcessProvider] Failed to determine if Path of Exile is run as admin successfully. We are going to assume that it is at this point.");
                return true;
            }
            finally
            {
                User32.CloseHandle(ph);
            }
        }

        public void Dispose()
        {
            WindowsHook?.Cancel();
        }
    }
}

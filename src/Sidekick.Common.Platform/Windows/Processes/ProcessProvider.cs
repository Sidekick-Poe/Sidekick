#pragma warning disable CA1806// Do not ignore method results
#pragma warning disable CA1416// Validate platform compatibility

using System.Diagnostics;
using System.Security.Principal;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Platform.Localization;
using Sidekick.Common.Platform.Windows.DllImport;

namespace Sidekick.Common.Platform.Windows.Processes;

public class ProcessProvider
(
    ILogger<ProcessProvider> logger,
    IApplicationService applicationService,
    ISidekickDialogs dialogService,
    IStringLocalizer<PlatformResources> resources
) : IProcessProvider, IDisposable
{
    private const string POE_PROCESS_STARTS_WITH = "PathOfExile";

    private static readonly List<string> PossibleProcessNames = new()
    {
        "PathOfExile",
        "PathOfExile_x64",
        "PathOfExile_KG",
        "PathOfExile_x64_KG",
        "PathOfExileSteam",
        "PathOfExile_x64Steam",
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

    private string? GetFocusedProcessName()
    {
        try
        {
            var hWnd = User32.GetForegroundWindow();
            if (hWnd == IntPtr.Zero)
            {
                logger.LogWarning("GetForegroundWindow returned a null handle.");
                return null;
            }

            User32.GetWindowThreadProcessId(hWnd, out var processId);
            if (processId <= 0) return null;

            using var process = Process.GetProcessById(processId);
            // logger.LogInformation("GetForegroundWindow returned " + process.ProcessName);
            return process.ProcessName;
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "[ProcessProvider] Failed to grab the focused process: {0}", e.Message);
            return null;
        }
    }

    /// <inheritdoc/>
    public bool IsPathOfExileInFocus
    {
        get
        {
            var focusedProcessName = GetFocusedProcessName();
            if (focusedProcessName == null) return false;

            return PossibleProcessNames.Contains(focusedProcessName, StringComparer.OrdinalIgnoreCase) ||
                   focusedProcessName.StartsWith(POE_PROCESS_STARTS_WITH);
        }
    }

    /// <inheritdoc/>
    public bool IsSidekickInFocus
    {
        get
        {
            var focusedProcessName = GetFocusedProcessName();
            if (focusedProcessName == null) return false;

            return string.Equals(focusedProcessName, "Sidekick", StringComparison.OrdinalIgnoreCase);
        }
    }

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
            try
            {
                if (!IsUserRunAsAdmin() && IsPathOfExileRunAsAdmin())
                {
                    await RestartAsAdmin();
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "[ProcessProvider] Exception occurred during admin check.");
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
        if (!await dialogService.OpenConfirmationModal(resources["RestartAsAdminText"]))
        {
            return;
        }

        try
        {
            using var p = new Process();
            p.StartInfo = new ProcessStartInfo
            {
                FileName = "Sidekick.exe",
                UseShellExecute = true,
                Verb = "runas",
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
            };

            if (p.Start())
            {
                logger.LogInformation("[ProcessProvider] Restarting application as administrator.");
            }
            else
            {
                logger.LogWarning("[ProcessProvider] Failed to start the Sidekick process as admin.");
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[ProcessProvider] Failed to restart the application as administrator.");
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
        var ph = IntPtr.Zero;

        try
        {
            User32.GetWindowThreadProcessId(User32.GetForegroundWindow(), out var processId);
            var proc = Process.GetProcessById(processId);

            User32.OpenProcessToken(proc.Handle, TOKEN_ALL_ACCESS, out ph);
            using var windowsIdentity = new WindowsIdentity(ph);
            if (windowsIdentity.Groups == null) return false;

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

                return true;
            }

            return false;
        }
        catch (Exception e)
        {
            logger.LogWarning(e, e.Message);
            logger.LogWarning(e, "[ProcessProvider] Failed to determine if Path of Exile is run as admin successfully. We are going to assume that it is at this point.");
            return true;
        }
        finally
        {
            if (ph != IntPtr.Zero) User32.CloseHandle(ph);
        }
    }

    public void Dispose()
    {
        WindowsHook?.Cancel();
    }
}

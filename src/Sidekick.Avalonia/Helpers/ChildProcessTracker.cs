using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Sidekick.Avalonia.Helpers;

/// <summary>
/// Ensures child processes are killed when the parent process exits,
/// even if the parent is forcibly terminated, by using a Win32 Job Object.
/// </summary>
internal sealed class ChildProcessTracker : IDisposable
{
    private nint jobHandle;
    private bool disposed;

    public ChildProcessTracker()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

        jobHandle = CreateJobObject(nint.Zero, null);
        if (jobHandle == nint.Zero) return;

        var info = new JobObjectBasicLimitInformation
        {
            LimitFlags = JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE
        };
        var extInfo = new JobObjectExtendedLimitInformation { BasicLimitInformation = info };
        int length = Marshal.SizeOf<JobObjectExtendedLimitInformation>();
        var extInfoPtr = Marshal.AllocHGlobal(length);
        try
        {
            Marshal.StructureToPtr(extInfo, extInfoPtr, false);
            SetInformationJobObject(jobHandle, JobObjectExtendedLimitInformationInt, extInfoPtr, (uint)length);
        }
        finally
        {
            Marshal.FreeHGlobal(extInfoPtr);
        }
    }

    /// <summary>
    /// Adds a process to the job object so it is killed when this tracker is disposed
    /// or when the parent process exits for any reason.
    /// </summary>
    public void Track(Process process)
    {
        if (jobHandle == nint.Zero) return;
        AssignProcessToJobObject(jobHandle, process.Handle);
    }

    public void Dispose()
    {
        if (disposed) return;
        disposed = true;
        if (jobHandle != nint.Zero)
        {
            CloseHandle(jobHandle);
            jobHandle = nint.Zero;
        }
    }

    #region P/Invoke

    private const int JobObjectExtendedLimitInformationInt = 9;
    private const uint JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 0x2000;

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern nint CreateJobObject(nint lpJobAttributes, string? lpName);

    [DllImport("kernel32.dll")]
    private static extern bool SetInformationJobObject(nint hJob, int jobObjectInfoClass, nint lpJobObjectInfo, uint cbJobObjectInfoLength);

    [DllImport("kernel32.dll")]
    private static extern bool AssignProcessToJobObject(nint hJob, nint hProcess);

    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(nint hObject);

    [StructLayout(LayoutKind.Sequential)]
    private struct JobObjectBasicLimitInformation
    {
        public long PerProcessUserTimeLimit, PerJobUserTimeLimit;
        public uint LimitFlags, MinimumWorkingSetSize, MaximumWorkingSetSize, ActiveProcessLimit;
        public nint Affinity;
        public uint PriorityClass, SchedulingClass;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct IoCounters
    {
        public ulong ReadOperationCount, WriteOperationCount, OtherOperationCount;
        public ulong ReadTransferCount, WriteTransferCount, OtherTransferCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct JobObjectExtendedLimitInformation
    {
        public JobObjectBasicLimitInformation BasicLimitInformation;
        public IoCounters IoInfo;
        public nint ProcessMemoryLimit, JobMemoryLimit, PeakProcessMemoryUsed, PeakJobMemoryUsed;
    }

    #endregion
}
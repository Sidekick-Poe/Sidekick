using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Initialization;
using Sidekick.Common.Ui.Overlay;

namespace Sidekick.Linux.Diagnostics;

public sealed class LinuxDiagnosticsService(
    ILogger<LinuxDiagnosticsService> logger,
    OverlayWidgetService widgetService) : IInitializableService, IDisposable
{
    private const string EnabledEnv = "SIDEKICK_DIAGNOSTICS";
    private const string IntervalEnv = "SIDEKICK_DIAGNOSTICS_INTERVAL_SEC";
    private const int DefaultIntervalSeconds = 10;

    private readonly Process process = Process.GetCurrentProcess();
    private Timer? timer;
    private TimeSpan lastCpuTime;
    private DateTimeOffset lastSampleTime;
    private long lastAllocatedBytes;
    private int sampling;

    public int Priority => 160;

    public Task Initialize()
    {
        if (!OperatingSystem.IsLinux() || !IsEnabled())
        {
            return Task.CompletedTask;
        }

        logger.LogInformation("[Diagnostics] Linux diagnostics enabled.");
        lastCpuTime = process.TotalProcessorTime;
        lastSampleTime = DateTimeOffset.UtcNow;
        lastAllocatedBytes = GC.GetTotalAllocatedBytes(false);

        timer = new Timer(_ => Sample(), null, TimeSpan.Zero, GetInterval());
        widgetService.WidgetsChanged += OnWidgetsChanged;
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        widgetService.WidgetsChanged -= OnWidgetsChanged;
        timer?.Dispose();
        timer = null;
    }

    private void OnWidgetsChanged()
    {
        if (timer == null)
        {
            return;
        }

        logger.LogInformation("[Diagnostics] Widgets changed: count={Count}", widgetService.Widgets.Count);
        Sample();
    }

    private void Sample()
    {
        if (Interlocked.Exchange(ref sampling, 1) == 1)
        {
            return;
        }

        try
        {
            process.Refresh();

            var now = DateTimeOffset.UtcNow;
            var cpuTime = process.TotalProcessorTime;
            var deltaCpu = cpuTime - lastCpuTime;
            var deltaWall = now - lastSampleTime;
            var cpuPercent = deltaWall.TotalMilliseconds <= 0
                ? 0
                : deltaCpu.TotalMilliseconds / (deltaWall.TotalMilliseconds * Environment.ProcessorCount) * 100.0;

            lastCpuTime = cpuTime;
            lastSampleTime = now;

            var allocatedBytes = GC.GetTotalAllocatedBytes(false);
            var deltaAllocated = allocatedBytes - lastAllocatedBytes;
            lastAllocatedBytes = allocatedBytes;

            var gcInfo = GC.GetGCMemoryInfo();
            var heapBytes = gcInfo.HeapSizeBytes;
            var totalMemory = GC.GetTotalMemory(false);

            logger.LogInformation(
                "[Diagnostics] cpu={CpuPercent:F1}% ws={WorkingSetMB:F1}MB private={PrivateMB:F1}MB heap={HeapMB:F1}MB gc_heap={GcHeapMB:F1}MB alloc_delta={AllocMB:F1}MB gc0={Gc0} gc1={Gc1} gc2={Gc2} threads={Threads}",
                cpuPercent,
                process.WorkingSet64 / 1024d / 1024d,
                process.PrivateMemorySize64 / 1024d / 1024d,
                heapBytes / 1024d / 1024d,
                totalMemory / 1024d / 1024d,
                deltaAllocated / 1024d / 1024d,
                GC.CollectionCount(0),
                GC.CollectionCount(1),
                GC.CollectionCount(2),
                process.Threads.Count);
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "[Diagnostics] Failed to sample process metrics.");
        }
        finally
        {
            Interlocked.Exchange(ref sampling, 0);
        }
    }

    private static bool IsEnabled()
    {
        var value = Environment.GetEnvironmentVariable(EnabledEnv);
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return value.Trim().ToLowerInvariant() switch
        {
            "1" => true,
            "true" => true,
            "yes" => true,
            "on" => true,
            _ => false
        };
    }

    private static TimeSpan GetInterval()
    {
        var value = Environment.GetEnvironmentVariable(IntervalEnv);
        if (int.TryParse(value, out var seconds) && seconds > 0)
        {
            return TimeSpan.FromSeconds(seconds);
        }

        return TimeSpan.FromSeconds(DefaultIntervalSeconds);
    }
}

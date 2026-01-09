using System.Drawing;
using System.Threading;
using Microsoft.Extensions.Logging;
using WebWindowNetCore;

namespace Sidekick.Linux.Platform;

internal sealed class WebWindowHost
{
    private readonly ILogger logger;
    private readonly string url;
    private readonly string title;
    private readonly int width;
    private readonly int height;
    private Thread? thread;

    public WebWindowHost(
        string url,
        string title,
        int width,
        int height,
        ILogger logger)
    {
        this.url = url;
        this.title = title;
        this.width = width;
        this.height = height;
        this.logger = logger;
    }

    public string Title => title;

    public bool IsRunning => thread?.IsAlive == true;

    public void Show()
    {
        if (thread != null)
        {
            return;
        }

        thread = new Thread(Run)
        {
            IsBackground = true,
            Name = "SidekickWebWindow",
        };
        thread.Start();
    }

    public void Close()
    {
        // WebWindowNetCore does not expose a programmatic close yet.
        // Users can close the window manually.
    }

    private void Run()
    {
        try
        {
            logger.LogInformation("[Linux/WebWindow] Starting WebWindowNetCore at {Url}", url);
            new WebView()
                .Url(url)
                .Title(title)
                .AppId("dev.sidekick.overlay")
                .InitialBounds(width, height)
                .WithoutNativeTitlebar()
                .BackgroundColor(Color.FromArgb(0, 0, 0, 0))
                .DefaultContextMenuDisabled()
                .Run();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Linux/WebWindow] Failed to start WebWindowNetCore overlay.");
        }
    }
}

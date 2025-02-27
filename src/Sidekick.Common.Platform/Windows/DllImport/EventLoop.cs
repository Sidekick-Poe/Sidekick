namespace Sidekick.Common.Platform.Windows.DllImport;

internal static class EventLoop
{
    public static CancellationTokenSource Run(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, User32.WinEventDelegate eventDelegate, uint idProcess, uint idThread, uint dwFlags)
    {
        var cancellationToken = new CancellationTokenSource();

        Task.Run(() =>
        {
            var handle = User32.SetWinEventHook(eventMin, eventMax, hmodWinEventProc, eventDelegate, idProcess, idThread, dwFlags);
            while (!cancellationToken.Token.IsCancellationRequested)
            {
                if (User32.PeekMessage(out var msg, IntPtr.Zero, 0, 0, PM_REMOVE))
                {
                    if (msg.Message == WM_QUIT)
                        break;

                    User32.TranslateMessage(ref msg);
                    User32.DispatchMessage(ref msg);
                }
                Thread.Sleep(200);
            }
            User32.UnhookWinEvent(handle);
        }, cancellationToken.Token);


        return cancellationToken;
    }

    const uint PM_REMOVE = 1;
    const uint WM_QUIT = 0x0012;
}

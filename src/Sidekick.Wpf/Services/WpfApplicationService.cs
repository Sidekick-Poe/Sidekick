using Sidekick.Common.Platform;

namespace Sidekick.Wpf.Services;

public class WpfApplicationService : IApplicationService
{
    public void Shutdown()
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            System.Windows.Application.Current.Shutdown();
        });
        Environment.Exit(0);
    }
}

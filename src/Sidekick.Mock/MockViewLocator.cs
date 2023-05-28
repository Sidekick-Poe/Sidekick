using System.Threading.Tasks;
using Sidekick.Common.Blazor.Views;

namespace Sidekick.Mock
{
    public class MockViewLocator : IViewLocator
    {
        public Task Open(string url)
        {
            return Task.CompletedTask;
        }

        public Task CloseAllOverlays()
        {
            return Task.CompletedTask;
        }

        public bool IsOverlayOpened()
        {
            return false;
        }

        public Task Close(SidekickView view)
        {
            return Task.CompletedTask;
        }

        public Task Initialize(SidekickView view)
        {
            return Task.CompletedTask;
        }

        public Task Minimize(SidekickView view)
        {
            return Task.CompletedTask;
        }

        public Task Maximize(SidekickView view)
        {
            return Task.CompletedTask;
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Sidekick.Common.Blazor;
using Sidekick.Common.Blazor.Views;

namespace Sidekick.Mock
{
    public class MockViewLocator : IViewLocator
    {
        public List<SidekickView> Views { get; } = new();

        public Task Open(string url)
        {
            return Task.CompletedTask;
        }

        public void CloseAllOverlays()
        {
            // Do nothing
        }

        public bool IsOverlayOpened()
        {
            return false;
        }
    }
}

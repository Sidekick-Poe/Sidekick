using System.Collections.Generic;
using System.Threading.Tasks;
using Sidekick.Common.Blazor;
using Sidekick.Common.Blazor.Views;

namespace Sidekick.Mock
{
    public class MockViewLocator : IViewLocator
    {
        private List<SidekickView> Views { get; set; } = new();

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

        public void Close(SidekickView view)
        {
            throw new System.NotImplementedException();
        }

        public void Add(SidekickView view)
        {
            Views.Add(view);
        }

        public void Remove(SidekickView view)
        {
            Views.Remove(view);
        }
    }
}

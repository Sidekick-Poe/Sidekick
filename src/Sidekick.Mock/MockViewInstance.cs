using System.Threading.Tasks;
using Sidekick.Common.Blazor.Views;

namespace Sidekick.Mock
{
    public class MockViewInstance : IViewInstance
    {
        public virtual Task Close()
        {
            return Task.CompletedTask;
        }

        public Task Initialize(string title, int width = 768, int height = 600, bool isOverlay = false, bool isModal = false, bool closeOnBlur = false)
        {
            return Task.CompletedTask;
        }
    }
}

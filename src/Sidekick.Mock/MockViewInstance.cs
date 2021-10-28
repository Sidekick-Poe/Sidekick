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

        public virtual Task Maximize()
        {
            return Task.CompletedTask;
        }

        public virtual Task Minimize()
        {
            return Task.CompletedTask;
        }

        public string Title { get; private set; }

        public Task Initialize(string title, int width = 768, int height = 600, bool isOverlay = false, bool isModal = false, bool closeOnBlur = false)
        {
            Title = title;
            return Task.CompletedTask;
        }

        public Task CloseAndOpen(string url)
        {
            return Task.CompletedTask;
        }
    }
}

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Sidekick.Common.Blazor.Views;

namespace Sidekick.Development.Views
{
    public class DevelopmentViewInstance : IViewInstance
    {
        private readonly NavigationManager navigationManager;

        public DevelopmentViewInstance(NavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
        }

        public virtual Task Close()
        {
            navigationManager.NavigateTo("/");
            return Task.CompletedTask;
        }

        public Task CloseAndOpen(string url)
        {
            navigationManager.NavigateTo(url);
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
    }
}

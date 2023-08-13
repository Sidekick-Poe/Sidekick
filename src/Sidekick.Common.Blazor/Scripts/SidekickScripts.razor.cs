using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Sidekick.Common.Blazor.Scripts
{
    /// <summary>
    /// Component that includes useful JS scripts.
    /// </summary>
    public partial class SidekickScripts : ComponentBase
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        private IJSObjectReference Module { get; set; }

        /// <inheritdoc/>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!firstRender)
            {
                return;
            }

            Module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Sidekick.Common.Blazor/Scripts/SidekickScripts.razor.js");
        }
    }
}

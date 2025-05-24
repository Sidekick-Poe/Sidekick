using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Sidekick.Common.Ui.Utilities;

/// <summary>
/// Used to initialize Flowbite components.
/// Necessary when rendering each component.
/// </summary>
public abstract class FlowbiteComponent : ComponentBase
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsRuntime.InvokeVoidAsync("initFlowbite");
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}

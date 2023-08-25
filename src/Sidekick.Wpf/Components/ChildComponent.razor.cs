using Microsoft.AspNetCore.Components;

namespace Sidekick.Wpf.Components;

public partial class ChildComponent : ComponentBase
{
    protected string Message { get; set; } = string.Empty;

    protected override void OnInitialized()
    {
        Message = $"ChildComponent initialized at {DateTime.Now.ToLongTimeString()}";
    }
}

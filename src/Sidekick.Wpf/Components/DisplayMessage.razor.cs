using Blazored.Modal;
using Microsoft.AspNetCore.Components;

namespace Sidekick.Wpf.Components;

public partial class DisplayMessage : ComponentBase
{
    [CascadingParameter]
    public BlazoredModalInstance BlazoredModal { get; set; }

    [Parameter]
    public string Message { get; set; }

    private async Task SubmitForm() => await BlazoredModal.CloseAsync();

    private async Task Cancel() => await BlazoredModal.CancelAsync();
}

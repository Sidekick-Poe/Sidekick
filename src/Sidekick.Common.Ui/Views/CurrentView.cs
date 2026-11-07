namespace Sidekick.Common.Ui.Views;

/// <summary>
/// The current view
/// </summary>
public class CurrentView(IViewLocator viewLocator)
{
    public SidekickViewType CurrentViewType { get; private set; }

    public void SetViewType(SidekickViewType type)
    {
        CurrentViewType = type;
    }

    public void Close()
    {
        viewLocator.Close(CurrentViewType);
    }
}

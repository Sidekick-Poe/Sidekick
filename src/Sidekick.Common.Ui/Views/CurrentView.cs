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

    public void SetSize(int width, int height)
    {
        viewLocator.SetSize(CurrentViewType, width, height, this);
    }

    public void ResetSize()
    {
        viewLocator.ResetSize(CurrentViewType, this);
    }
}

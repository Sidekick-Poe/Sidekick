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

    public void Maximize()
    {
        viewLocator.Maximize(CurrentViewType);
    }

    public void Minimize()
    {
        viewLocator.Minimize(CurrentViewType);
    }

    public void StartMoving(int pageX, int pageY)
    {
        viewLocator.StartMoving(CurrentViewType, pageX, pageY);
    }

    public void StopMoving()
    {
        viewLocator.StopMoving(CurrentViewType);
    }
}

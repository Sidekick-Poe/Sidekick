namespace Sidekick.Common.Ui.Views;

/// <summary>
/// The current view
/// </summary>
public class CurrentView(IViewLocator viewLocator)
{
    /// <summary>
    /// The current view type
    /// </summary>
    public SidekickViewType CurrentViewType { get; private set; }

    /// <summary>
    /// Set the current view type
    /// </summary>
    /// <param name="type">The type of view </param>
    public void SetViewType(SidekickViewType type)
    {
        CurrentViewType = type;
    }

    /// <summary>
    /// Close the current view
    /// </summary>
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

    public void StartDragging(int pageX, int pageY)
    {
        throw new NotImplementedException();
    }

    public void StopDragging()
    {
        throw new NotImplementedException();
    }
}

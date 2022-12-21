namespace Sidekick.Maui;

public partial class App : Application
{
    public App(MauiBlazorInterop mauiBlazorInterop)
    {
        InitializeComponent();

#if DEBUG
        mauiBlazorInterop.StartPage = "/setup";
#else
        mauiBlazorInterop.StartPage = "/update";
#endif

        MainPage = new MainPage();
    }
}

namespace Sidekick.Maui;

public partial class App : Application
{
    public App(MauiBlazorInterop mauiBlazorInterop)
    {
        InitializeComponent();

        mauiBlazorInterop.StartPage = "/setup";

        MainPage = new MainPage();
    }
}

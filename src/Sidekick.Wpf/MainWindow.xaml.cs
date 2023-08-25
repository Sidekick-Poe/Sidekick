using System.Windows;

namespace Sidekick.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        Resources.Add("services", App.ServiceProvider);
        InitializeComponent();
    }
}

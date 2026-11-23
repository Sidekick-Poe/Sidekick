using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Microsoft.Extensions.Localization;
using Sidekick.Avalonia.Localization;
using Sidekick.Common.Platform.Linux;
using Sidekick.Common.Platform.Linux.Dependencies;
using Sidekick.Common.Ui.Localization;

namespace Sidekick.Avalonia;

/// <summary>
/// Lists the packages Sidekick needs on Linux, with commands the user can select or copy to install them.
/// Shown instead of starting Sidekick, so it cannot use the web interface, which may be the missing package.
/// </summary>
public partial class LinuxDependenciesWindow : Window
{
    private const int WIDTH = 560;

    private readonly IStringLocalizer<UiResources> resources;

    private readonly IStringLocalizer<AvaloniaResources> avaloniaResources;

    public LinuxDependenciesWindow(IReadOnlyList<IDependency> missingDependencies, IStringLocalizer<UiResources> resources, IStringLocalizer<AvaloniaResources> avaloniaResources)
    {
        this.resources = resources;
        this.avaloniaResources = avaloniaResources;

        Title = "Sidekick";
        SizeToContent = SizeToContent.Height;
        Width = WIDTH;
        Background = new SolidColorBrush(Color.FromRgb(12, 10, 10));
        WindowDecorations = WindowDecorations.None;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        RequestedThemeVariant = ThemeVariant.Dark;
        CanResize = false;

        InitializeComponent();

        TitleTextBlock.Text = avaloniaResources.Linux_Dependencies_Title;
        IntroductionTextBlock.Text = avaloniaResources.Linux_Dependencies_Introduction;
        InstructionsTextBlock.Text = avaloniaResources.Linux_Dependencies_Instructions;
        CloseButton.Content = resources.Close;

        foreach (var dependency in missingDependencies)
        {
            DependenciesPanel.Children.Add(new TextBlock
            {
                FontFamily = (FontFamily)this.FindResource("FontinFont")!,
                FontSize = 16,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                Text = dependency.Description,
            });
        }

        var commands = LinuxDependencyChecker.GetInstallCommands(missingDependencies);
        foreach (var command in commands)
        {
            if (commands.Count > 1)
            {
                CommandsPanel.Children.Add(new TextBlock
                {
                    FontFamily = (FontFamily)this.FindResource("FontinFont")!,
                    FontSize = 14,
                    Foreground = new SolidColorBrush(Color.FromRgb(168, 162, 158)),
                    Margin = new(0, 4, 0, 0),
                    Text = command.Distribution,
                });
            }

            CommandsPanel.Children.Add(CreateCommandRow(command.Command));
        }

        Opened += (_, _) => Dispatcher.UIThread.Post(() =>
        {
            Activate();
            CloseButton.Focus();
        }, DispatcherPriority.ApplicationIdle);
    }

    private Control CreateCommandRow(string command)
    {
        var textBox = new TextBox
        {
            Text = command,
            IsReadOnly = true,
            FontFamily = (FontFamily)this.FindResource("MonospaceFont")!,
            FontSize = 14,
            VerticalContentAlignment = VerticalAlignment.Center,
        };

        var copyButton = new Button
        {
            Content = resources.Copy,
            FontFamily = (FontFamily)this.FindResource("FontinFont")!,
            Margin = new(8, 0, 0, 0),
            MinWidth = 80,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Stretch,
            VerticalContentAlignment = VerticalAlignment.Center,
        };
        copyButton.Classes.Add("secondary");
        copyButton.Click += async (_, _) =>
        {
            if (Clipboard == null) return;

            await Clipboard.SetTextAsync(command);
            copyButton.Content = avaloniaResources.Copied;
            await Task.Delay(TimeSpan.FromSeconds(2));
            copyButton.Content = resources.Copy;
        };

        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto") };
        grid.Children.Add(textBox);
        Grid.SetColumn(copyButton, 1);
        grid.Children.Add(copyButton);
        return grid;
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}

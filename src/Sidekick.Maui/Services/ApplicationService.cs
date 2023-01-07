using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Sidekick.Common.Platform;

namespace Sidekick.Maui.Services
{
    /// Implementation of the IViewInstance interface.
    public class ApplicationService : IApplicationService
    {
        public void Shutdown()
        {
            Application.Current.Quit();
            Environment.Exit(0);
        }

        public Task OpenConfirmationNotification(string message, string title = null, Func<Task> onYes = null, Func<Task> onNo = null)
        {
            throw new NotImplementedException();
        }

        public void ShowToast(string message)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var options = new SnackbarOptions
            {
                BackgroundColor = Colors.Red,
                TextColor = Colors.Green,
                ActionButtonTextColor = Colors.Yellow,
                CornerRadius = new CornerRadius(10),
                Font = Microsoft.Maui.Font.SystemFontOfSize(14),
                ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(14),
                CharacterSpacing = 0.5
            };

            // var snackbar = Snackbar.Make(
            //     message,
            //     duration: TimeSpan.FromSeconds(3));
            //
            // _ = snackbar.Show(cancellationTokenSource.Token);

            var toast = Toast.Make(message);

            _ = toast.Show(cancellationTokenSource.Token);
        }
    }
}

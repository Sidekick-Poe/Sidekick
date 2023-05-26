using System;
using System.Threading.Tasks;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Hosting;
using Sidekick.Common.Platform;

namespace Sidekick.Electron.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public ApplicationService(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task OpenConfirmationNotification(string message, string title = null, Func<Task> onYes = null, Func<Task> onNo = null)
        {
            var options = new MessageBoxOptions(message)
            {
                Buttons = new[] { "Yes", "No" },
                CancelId = 1,
                DefaultId = 0,
                Title = title,
                Icon = $"{webHostEnvironment.ContentRootPath}Assets/icon.png"
            };

            var result = await ElectronNET.API.Electron.Dialog.ShowMessageBoxAsync(options);
            if (result.Response == 0)
            {
                await onYes.Invoke();
            }
            else
            {
                await onNo.Invoke();
            }
        }

        public void Shutdown()
        {
            ElectronNET.API.Electron.App.Exit();
        }
    }
}

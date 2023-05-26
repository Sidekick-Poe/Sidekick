using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.Poe.Parser.Patterns;
using Sidekick.Apis.Poe.Pseudo;
using Sidekick.Apis.PoeNinja;
using Sidekick.Apis.PoeWiki;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Localization;
using Sidekick.Common.Platform;
using Sidekick.Common.Platform.Tray;
using Sidekick.Common.Settings;
using Sidekick.Modules.Initialization.Localization;

namespace Sidekick.Modules.Initialization.Pages
{
    public partial class Initialization : ComponentBase
    {
        [Inject] private InitializationResources Resources { get; set; }
        [Inject] private ISettings Settings { get; set; }
        [Inject] private ILogger<Initialization> Logger { get; set; }
        [Inject] private IViewInstance ViewInstance { get; set; }
        [Inject] private IViewLocator ViewLocator { get; set; }
        [Inject] private IProcessProvider ProcessProvider { get; set; }
        [Inject] private IKeyboardProvider KeyboardProvider { get; set; }
        [Inject] private IKeybindProvider KeybindProvider { get; set; }
        [Inject] private IParserPatterns ParserPatterns { get; set; }
        [Inject] private IModifierProvider ModifierProvider { get; set; }
        [Inject] private IPseudoModifierProvider PseudoModifierProvider { get; set; }
        [Inject] private IItemMetadataProvider ItemMetadataProvider { get; set; }
        [Inject] private IItemStaticDataProvider ItemStaticDataProvider { get; set; }
        [Inject] private IGameLanguageProvider GameLanguageProvider { get; set; }
        [Inject] private IApplicationService ApplicationService { get; set; }
        [Inject] private IUILanguageProvider UILanguageProvider { get; set; }
        [Inject] private IEnglishModifierProvider EnglishModifierProvider { get; set; }
        [Inject] private IPoeNinjaClient PoeNinjaClient { get; set; }
        [Inject] private ITrayProvider TrayProvider { get; set; }
        [Inject] private IPoeWikiDataProvider PoeWikiDataProvider { get; set; }

        private int Count { get; set; } = 0;
        private int Completed { get; set; } = 0;
        private string Title { get; set; }
        private int Percentage { get; set; }
        private bool Error { get; set; }

        public Task InitializationTask { get; set; }

        protected override async Task OnInitializedAsync()
        {
            InitializationTask = Handle();
            await ViewInstance.Initialize("Initialize", width: 400, height: 230, isModal: true);
            await base.OnInitializedAsync();
            await InitializationTask;
        }

        public async Task Handle()
        {
            try
            {
                Completed = 0;
                Count = 14;

                // Report initial progress
                await ReportProgress();

                // Languages
                await Run(() => UILanguageProvider.Set(Settings.Language_UI));
                await Run(() => GameLanguageProvider.SetLanguage(Settings.Language_Parser));

                await Run(() => ParserPatterns.Initialize());
                await Run(() => ItemMetadataProvider.Initialize());
                await Run(() => ItemStaticDataProvider.Initialize());
                await Run(() => EnglishModifierProvider.Initialize());
                await Run(() => ModifierProvider.Initialize());
                await Run(() => PseudoModifierProvider.Initialize());
                await Run(() => PoeNinjaClient.Initialize());
                await Run(() => KeybindProvider.Initialize());
                await Run(() => ProcessProvider.Initialize());
                await Run(() => KeyboardProvider.Initialize());
                await Run(() => PoeWikiDataProvider.Initialize());
                await Run(() => InitializeTray());

                // If we have a successful initialization, we delay for half a second to show the
                // "Ready" label on the UI before closing the view
                Completed = Count;
                await ReportProgress();
                await Task.Delay(5000);
                await ViewInstance.Close();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                Error = true;
            }
        }

        private async Task Run(Func<Task> func)
        {
            // Send the command
            await func.Invoke();

            // Make sure that after all handlers run, the Completed count is updated
            Completed += 1;

            // Report progress
            await ReportProgress();
        }

        private async Task Run(Action action)
        {
            // Send the command
            action.Invoke();

            // Make sure that after all handlers run, the Completed count is updated
            Completed += 1;

            // Report progress
            await ReportProgress();
        }

        private Task ReportProgress()
        {
            return InvokeAsync(() =>
            {
                Percentage = Count == 0 ? 0 : Completed * 100 / Count;
                if (Percentage >= 100)
                {
                    Title = Resources.Ready;
                    Percentage = 100;
                }
                else
                {
                    Title = Resources.Title(Completed, Count);
                }

                StateHasChanged();
                return Task.Delay(100);
            });
        }

        private void InitializeTray()
        {
            var menuItems = new List<TrayMenuItem>();

            menuItems.AddRange(new List<TrayMenuItem>()
            {
                new ()
                {
                    Label = "Sidekick - " + typeof(StartupExtensions).Assembly.GetName().Version.ToString(),
                    Disabled = true,
                },
                new ()
                {
                    Label = "Cheatsheets",
                    OnClick = () => ViewLocator.Open("/cheatsheets"),
                },
                new ()
                {
                    Label = "About",
                    OnClick = () => ViewLocator.Open("/about"),
                },
                new ()
                {
                    Label = "Settings",
                    OnClick = () => ViewLocator.Open("/settings"),
                },
                new ()
                {
                    Label = "Exit",
                    OnClick = () => { ApplicationService.Shutdown(); return Task.CompletedTask; },
                },
            });

            TrayProvider.Initialize(menuItems);
        }

        public void Exit()
        {
            ApplicationService.Shutdown();
        }
    }
}

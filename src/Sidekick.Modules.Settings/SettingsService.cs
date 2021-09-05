using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Sidekick.Common;
using Sidekick.Common.Blazor.Views;
using Sidekick.Common.Cache;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Localization;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.Settings
{
    public class SettingsService : ISettingsService
    {
        public const string FileName = "Sidekick_settings.json";
        private readonly ISettings settings;
        private readonly IGameLanguageProvider gameLanguageProvider;
        private readonly ICacheProvider cacheProvider;
        private readonly IUILanguageProvider uILanguageProvider;
        private readonly IViewLocator viewLocator;

        public SettingsService(
            ISettings settings,
            IGameLanguageProvider gameLanguageProvider,
            ICacheProvider cacheProvider,
            IUILanguageProvider uILanguageProvider,
            IViewLocator viewLocator)
        {
            this.settings = settings;
            this.gameLanguageProvider = gameLanguageProvider;
            this.cacheProvider = cacheProvider;
            this.uILanguageProvider = uILanguageProvider;
            this.viewLocator = viewLocator;
        }

        public async Task Save(string property, object value)
        {
            var propertyType = settings.GetType().GetProperty(property);
            var propertyValue = propertyType.GetValue(settings);

            if (propertyValue != null && value != null && value.Equals(propertyValue))
            {
                return;
            }

            var newSettings = new Settings();
            settings.CopyValuesTo(newSettings);
            propertyType.SetValue(newSettings, value);
            await Save(newSettings);
        }

        public async Task Save(ISettings newSettings)
        {
            var leagueHasChanged = settings.LeagueId != newSettings.LeagueId;
            var languageHasChanged = settings.Language_Parser != newSettings.Language_Parser;

            if (settings.Language_UI != newSettings.Language_UI)
            {
                uILanguageProvider.Set(newSettings.Language_UI);
            }
            if (languageHasChanged)
            {
                gameLanguageProvider.SetLanguage(newSettings.Language_Parser);
            }

            newSettings.CopyValuesTo(settings);

            var json = JsonSerializer.Serialize(settings);
            var defaults = JsonSerializer.Serialize(new Settings());
            var filePath = SidekickPaths.GetDataFilePath(FileName);

            using var fileStream = File.Create(filePath);
            using var writer = new Utf8JsonWriter(fileStream, options: new JsonWriterOptions
            {
                Indented = true
            });
            using var document = JsonDocument.Parse(json, new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip
            });
            using var defaultsDocument = JsonDocument.Parse(defaults, new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip
            });

            var root = document.RootElement;
            var defaultsRoot = defaultsDocument.RootElement;

            if (root.ValueKind == JsonValueKind.Object)
            {
                writer.WriteStartObject();
            }
            else
            {
                return;
            }

            foreach (var property in root.EnumerateObject())
            {
                if (defaultsRoot.GetProperty(property.Name).ToString() == property.Value.ToString())
                {
                    continue;
                }

                property.WriteTo(writer);
            }

            writer.WriteEndObject();
            writer.Flush();

            if (writer.BytesCommitted == 0)
            {
                File.Delete(filePath);
            }

            if (languageHasChanged || leagueHasChanged)
            {
                cacheProvider.Clear();
                await viewLocator.Open("/initialize");
            }
        }
    }
}

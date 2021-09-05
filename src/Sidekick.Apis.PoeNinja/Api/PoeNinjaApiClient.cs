using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.PoeNinja.Api.Models;
using Sidekick.Apis.PoeNinja.Models;
using Sidekick.Apis.PoeNinja.Repository;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.PoeNinja.Api
{
    /// <summary>
    /// https://poe.ninja/swagger
    /// </summary>
    public class PoeNinjaApiClient : IPoeNinjaApiClient
    {
        private readonly static Uri POE_NINJA_API_BASE_URL = new("https://poe.ninja/api/data/");
        /// <summary>
        /// Poe.ninja uses its own language codes.
        /// </summary>
        internal readonly static Dictionary<string, string> POE_NINJA_LANGUAGE_CODES = new()
        {
            { "de", "ge" }, // German.
            { "en", "en" }, // English.
            { "es", "es" }, // Spanish.
            { "fr", "fr" }, // French.
            { "kr", "ko" }, // Korean.
            { "pt", "pt" }, // Portuguese.
            { "ru", "ru" }, // Russian.
            { "th", "th" }, // Thai.
        };
        private readonly HttpClient client;
        private readonly ILogger logger;
        private readonly IGameLanguageProvider gameLanguageProvider;
        private readonly ISettings settings;
        private readonly IPoeNinjaRepository poeNinjaRepository;
        private readonly JsonSerializerOptions options;

        private string LanguageCode
        {
            get
            {
                if (POE_NINJA_LANGUAGE_CODES.TryGetValue(gameLanguageProvider.Language.LanguageCode, out var languageCode))
                {
                    return languageCode;
                }
                return string.Empty;
            }
        }

        public PoeNinjaApiClient(
            IHttpClientFactory httpClientFactory,
            ILogger<PoeNinjaClient> logger,
            IGameLanguageProvider gameLanguageProvider,
            ISettings settings,
            IPoeNinjaRepository poeNinjaRepository)
        {
            this.logger = logger;
            this.gameLanguageProvider = gameLanguageProvider;
            this.settings = settings;
            this.poeNinjaRepository = poeNinjaRepository;
            options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true,
            };
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

            client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Sidekick");
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("Sidekick");
        }

        public async Task<List<NinjaPrice>> FetchItems(ItemType itemType)
        {
            var url = new Uri($"{POE_NINJA_API_BASE_URL}itemoverview?league={settings.LeagueId}&type={itemType}&language={LanguageCode}");

            try
            {
                var response = await client.GetAsync(url);
                var responseStream = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<PoeNinjaQueryResult<PoeNinjaItem>>(responseStream, options);
                var items = result.Lines
                    .Select(x => new NinjaPrice()
                    {
                        Corrupted = x.Corrupted,
                        Price = x.ChaosValue,
                        LastUpdated = DateTimeOffset.Now,
                        Name = x.Name,
                        MapTier = x.MapTier,
                        GemLevel = x.GemLevel,
                    })
                    .ToList();

                await poeNinjaRepository.SavePrices(itemType, items);
                await poeNinjaRepository.SaveTranslations(itemType, result.Language.Translations
                    .Where(y => !y.Value.Contains("."))
                    .Distinct()
                    .Select(x => new NinjaTranslation()
                    {
                        English = x.Key,
                        Translation = x.Value,
                    })
                    .ToList());

                return items;
            }
            catch (Exception)
            {
                logger.LogInformation("Could not fetch {itemType} from poe.ninja", itemType);
            }

            return null;
        }

        public async Task<List<NinjaPrice>> FetchCurrencies(CurrencyType currencyType)
        {
            var url = new Uri($"{POE_NINJA_API_BASE_URL}currencyoverview?league={settings.LeagueId}&type={currencyType}&language={LanguageCode}");

            try
            {
                var response = await client.GetAsync(url);
                var responseStream = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<PoeNinjaQueryResult<PoeNinjaCurrency>>(responseStream, options);
                var items = result.Lines
                    .Select(x => new NinjaPrice()
                    {
                        Corrupted = false,
                        Price = x.Receive.Value,
                        LastUpdated = DateTimeOffset.Now,
                        Name = x.CurrencyTypeName,
                    })
                    .ToList();

                await poeNinjaRepository.SavePrices(currencyType, items);
                await poeNinjaRepository.SaveTranslations(currencyType, result.Language.Translations
                    .Where(y => !y.Value.Contains("."))
                    .Distinct()
                    .Select(x => new NinjaTranslation()
                    {
                        English = x.Key,
                        Translation = x.Value,
                    })
                    .ToList());

                return items;
            }
            catch
            {
                logger.LogInformation("Could not fetch {currency} from poe.ninja", currencyType);
            }

            return null;
        }
    }
}

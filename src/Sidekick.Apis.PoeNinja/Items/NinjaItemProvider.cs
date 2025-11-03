using System.Text.Json;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Apis.PoeNinja.Items.Models;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.PoeNinja.Items;

public class NinjaItemProvider(ISettingsService settingsService) : INinjaItemProvider
{
    private Dictionary<string, NinjaPage> Items { get; set; } = [];

    public int Priority => 100;

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        var dataFilePath = Path.Combine(AppContext.BaseDirectory, "wwwroot/data/" + NinjaPageProvider.GetFileName(game));
        if (!File.Exists(dataFilePath))
        {
            return;
        }

        await using var fileStream = File.OpenRead(dataFilePath);
        var result = await JsonSerializer.DeserializeAsync<List<NinjaPageItem>>(fileStream, NinjaClient.JsonSerializerOptions);
        if (result != null)
        {
            Items = result.ToDictionary(x => x.InvariantId, x => x.Page);
        }
    }

    public NinjaPage? GetPage(string invariantId) => Items.GetValueOrDefault(invariantId);
}

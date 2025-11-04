using System.Text.Json;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Apis.PoeNinja.Items.Models;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.PoeNinja.Items;

public class NinjaItemProvider(ISettingsService settingsService) : INinjaItemProvider
{
    private Dictionary<string, NinjaPage> Items { get; } = [];

    public int Priority => 100;

    private GameType Game { get; set; }

    public async Task Initialize()
    {
        Game = await settingsService.GetGame();
        var dataFilePath = Path.Combine(AppContext.BaseDirectory, "wwwroot/data/" + NinjaPageProvider.GetFileName(Game));
        if (!File.Exists(dataFilePath)) return;

        Items.Clear();

        await using var fileStream = File.OpenRead(dataFilePath);
        var result = await JsonSerializer.DeserializeAsync<List<NinjaPageItem>>(fileStream, NinjaClient.JsonSerializerOptions);
        if (result == null) return;

        foreach (var item in result)
        {
            if (string.IsNullOrEmpty(item.Name)) continue;
            Items.TryAdd(item.Name, item.Page);
        }
    }

    public NinjaPage? GetPage(string? invariant)
    {
        if (string.IsNullOrEmpty(invariant)) return null;
        var page = Items.GetValueOrDefault(invariant);
        if(page != null) return page;

        if (Game == GameType.PathOfExile && invariant == "chaos")
        {
            return new("Currency", "currency", true, true);
        }

        return null;
    }
}

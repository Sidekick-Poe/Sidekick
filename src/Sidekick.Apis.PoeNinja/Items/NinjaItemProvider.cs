using System.Text.Json;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Apis.PoeNinja.Items.Models;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.PoeNinja.Items;

public class NinjaItemProvider(ISettingsService settingsService) : INinjaItemProvider
{
    private Dictionary<string, NinjaPage> Items { get; } = [];

    public int Priority => 100;

    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        var dataFilePath = Path.Combine(AppContext.BaseDirectory, "wwwroot/data/" + NinjaPageProvider.GetFileName(game));
        if (!File.Exists(dataFilePath)) return;

        Items.Clear();

        await using var fileStream = File.OpenRead(dataFilePath);
        var result = await JsonSerializer.DeserializeAsync<List<NinjaPageItem>>(fileStream, NinjaClient.JsonSerializerOptions);
        if (result == null) return;

        foreach (var item in result)
        {
            Console.WriteLine($"Item: {item.Name}, Page: {item.Page}");
            if (string.IsNullOrEmpty(item.Name))
            {
                Console.WriteLine("Item skipped due to empty name!");
            }
            else
            {
                Items.TryAdd(item.Name, item.Page);
            }
            // if (string.IsNullOrEmpty(item.Name)) continue;
            // Items.TryAdd(item.Name, item.Page);
        }
    }

    public NinjaPage? GetPage(string? invariant)
    {
        if (string.IsNullOrEmpty(invariant)) return null;
        return Items.GetValueOrDefault(invariant);
    }
}

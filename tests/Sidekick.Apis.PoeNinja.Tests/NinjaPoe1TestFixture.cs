using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Tests.Poe1English;
using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Apis.PoeNinja.Exchange;
using Sidekick.Apis.PoeNinja.Stash;
using Sidekick.Apis.PoeNinja.Tests.Mocks;
using Sidekick.Game.Parser;
using Sidekick.Game.Parser.Items;
using Sidekick.Game.Providers;
using Xunit;

namespace Sidekick.Apis.PoeNinja.Tests;

public class NinjaPoe1TestFixture : Poe1EnglishFixture
{
    public INinjaExchangeProvider NinjaExchangeProvider { get; private set; } = null!;
    public INinjaStashProvider NinjaStashProvider { get; private set; } = null!;
    public ItemDefinitionParser ItemDefinitionParser { get; private set; } = null!;
    public ILogger Logger { get; private set; } = null!;
    public NinjaProvider NinjaProvider { get; private set; } = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        NinjaExchangeProvider = TestContext.Services.GetRequiredService<INinjaExchangeProvider>();
        NinjaStashProvider = TestContext.Services.GetRequiredService<INinjaStashProvider>();
        ItemDefinitionParser = TestContext.Services.GetRequiredService<ItemDefinitionParser>();
        Logger = TestContext.Services.GetRequiredService<ILogger<NinjaPoe1TestFixture>>();
        NinjaProvider = TestContext.Services.GetRequiredService<NinjaProvider>();
    }

    protected override void RegisterServices(IServiceCollection services)
    {
        base.RegisterServices(services);

        services.AddSingleton<INinjaClient, TestNinjaClient>();
    }

    public void AssertStash(Item item, string expectedDetailsId)
    {
        // poe.ninja drops items that are no longer traded, so only those are skipped. An item still on poe.ninja must be linked to its definition.
        if (!NinjaProvider.StashItems.ContainsKey(expectedDetailsId))
        {
            Logger.LogWarning($"Item {item.Name} {item.Type} is no longer on poe.ninja with the details id {expectedDetailsId}");
            return;
        }

        Assert.True(item.InvariantDefinition?.NinjaStashItems?.Any(x => x.DetailsId == expectedDetailsId) == true,
                    $"Item {item.Name} {item.Type} is on poe.ninja with the details id {expectedDetailsId}, but its definition is not linked to it.");

        var results = NinjaStashProvider.GetDefinitions(item);
        Assert.Single(results);

        var result = results[0];
        Assert.Equal(expectedDetailsId, result.DetailsId);
    }
}

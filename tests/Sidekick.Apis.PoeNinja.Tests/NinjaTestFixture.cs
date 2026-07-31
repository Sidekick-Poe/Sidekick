using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Tests.Poe1English;
using Sidekick.Apis.Poe.Trade.Parser.Definition;
using Sidekick.Apis.Poe.Trade.Trade.Models;
using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Apis.PoeNinja.Exchange;
using Sidekick.Apis.PoeNinja.Stash;
using Sidekick.Apis.PoeNinja.Tests.Mocks;
using Sidekick.Data.Items;
using Xunit;

namespace Sidekick.Apis.PoeNinja.Tests;

public class NinjaTestFixture : Poe1EnglishFixture
{
    public INinjaExchangeProvider NinjaExchangeProvider { get; private set; } = null!;
    public INinjaStashProvider NinjaStashProvider { get; private set; } = null!;
    public IItemDefinitionParser ItemDefinitionParser { get; private set; } = null!;
    public ILogger Logger { get; private set; } = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        NinjaExchangeProvider = TestContext.Services.GetRequiredService<INinjaExchangeProvider>();
        NinjaStashProvider = TestContext.Services.GetRequiredService<INinjaStashProvider>();
        ItemDefinitionParser = TestContext.Services.GetRequiredService<IItemDefinitionParser>();
        Logger = TestContext.Services.GetRequiredService<ILogger<NinjaTestFixture>>();
    }

    protected override void RegisterServices(IServiceCollection services)
    {
        base.RegisterServices(services);

        services.AddSingleton<INinjaClient, TestNinjaClient>();
    }

    public void AssertApiItem(ApiItem item, string expectedDetailsId)
    {
        var itemDefinition = ItemDefinitionParser.Get(item);
        var invariantDefinition = itemDefinition?.InvariantKey != null ? ItemDefinitionParser.InvariantDictionary.GetValueOrDefault(itemDefinition.InvariantKey) : null;
        if (invariantDefinition?.NinjaItems == null || invariantDefinition.NinjaItems.All(x => x.Stash?.DetailsId != expectedDetailsId))
        {
            Logger.LogWarning($"Item {item.Name} {item.Type} does not have expected details id {expectedDetailsId}");
            return;
        }

        var results = NinjaStashProvider.GetDefinitions(invariantDefinition, item);
        Assert.Single(results);
        Assert.Equal(expectedDetailsId, results[0].Stash?.DetailsId);
    }

    public void AssertStash(Item item, string expectedDetailsId)
    {
        if (item.Invariant.NinjaItems == null || item.Invariant.NinjaItems.All(x => x.Stash?.DetailsId != expectedDetailsId))
        {
            Logger.LogWarning($"Item {item.Name} {item.Type} does not have expected details id {expectedDetailsId}");
            return;
        }

        var results = NinjaStashProvider.GetDefinitions(item);
        Assert.Single(results);

        var result = results[0];
        Assert.Equal(expectedDetailsId, result.Stash?.DetailsId);
    }

    public void AssertExchange(Item item, string expectedDetailsId)
    {
        if (item.Invariant.NinjaItems != null && item.Invariant.NinjaItems.All(x => x.Exchange?.DetailsId != expectedDetailsId))
        {
            Logger.LogWarning($"Item {item.Name} {item.Type} does not have expected details id {expectedDetailsId}");
            return;
        }

        var result = NinjaExchangeProvider.GetDefinition(item.Invariant);
        Assert.Equal(expectedDetailsId, result?.Exchange?.DetailsId);
    }
}

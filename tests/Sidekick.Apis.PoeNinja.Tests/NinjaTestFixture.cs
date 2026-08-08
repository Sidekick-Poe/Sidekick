using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Tests.Poe1English;
using Sidekick.Apis.Poe.Trade.Parser.Definition;
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

    public void AssertStash(Item item, string expectedDetailsId)
    {
        if (item.InvariantTradeItem?.NinjaItems == null || item.InvariantTradeItem.NinjaItems.All(x => x.DetailsId != expectedDetailsId))
        {
            Logger.LogWarning($"Item {item.Name} {item.Type} does not have expected details id {expectedDetailsId}");
            return;
        }

        var results = NinjaStashProvider.GetDefinitions(item);
        Assert.Single(results);

        var result = results[0];
        Assert.Equal(expectedDetailsId, result.DetailsId);
    }
}

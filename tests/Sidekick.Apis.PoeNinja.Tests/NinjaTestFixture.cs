using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Tests.Poe1English;
using Sidekick.Apis.Poe.Trade.Parser.Definition;
using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Apis.PoeNinja.Exchange;
using Sidekick.Apis.PoeNinja.Stash;
using Sidekick.Apis.PoeNinja.Tests.Mocks;

namespace Sidekick.Apis.PoeNinja.Tests;

public class NinjaTestFixture : Poe1EnglishFixture
{
    public INinjaExchangeProvider NinjaExchangeProvider { get; private set; } = null!;
    public INinjaStashProvider NinjaStashProvider { get; private set; } = null!;
    public IItemDefinitionParser ItemDefinitionParser { get; private set; } = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        NinjaExchangeProvider = TestContext.Services.GetRequiredService<INinjaExchangeProvider>();
        NinjaStashProvider = TestContext.Services.GetRequiredService<INinjaStashProvider>();
        ItemDefinitionParser = TestContext.Services.GetRequiredService<IItemDefinitionParser>();
    }

    protected override void RegisterServices(IServiceCollection services)
    {
        base.RegisterServices(services);

        services.AddSingleton<INinjaClient, TestNinjaClient>();
    }
}

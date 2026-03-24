using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Sidekick.Apis.Poe.Tests.Poe1English;
using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Apis.PoeNinja.Exchange;
using Sidekick.Apis.PoeNinja.Exchange.Models;
using Sidekick.Apis.PoeNinja.Stash;
using Sidekick.Apis.PoeNinja.Stash.Models;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.Builder;

namespace Sidekick.Apis.PoeNinja.Tests;

public class NinjaTestFixture : Poe1EnglishFixture
{
    public INinjaExchangeProvider NinjaExchangeProvider { get; private set; } = null!;
    public INinjaStashProvider NinjaStashProvider { get; private set; } = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        NinjaExchangeProvider = TestContext.Services.GetRequiredService<INinjaExchangeProvider>();
        NinjaStashProvider = TestContext.Services.GetRequiredService<INinjaStashProvider>();
    }

    protected override void RegisterServices(IServiceCollection services)
    {
        base.RegisterServices(services);

        var ninjaClientMock = new Mock<INinjaClient>();

        // Mocking Fetch for NinjaExchangeOverview
        ninjaClientMock.Setup(x => x.Fetch<NinjaExchangeOverview>(It.IsAny<GameType>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string?>>()))
            .Returns<GameType, string, Dictionary<string, string?>>(async (game, path, parameters) =>
            {
                var type = parameters?["type"];
                if (type == null) return null;
                var dataProvider = services.BuildServiceProvider().GetRequiredService<RawDataProvider>();
                var filePath = $"{game.GetValueAttribute()}/raw/ninja/{type}.json";
                return await dataProvider.Read<NinjaExchangeOverview>(filePath);
            });

        // Mocking Fetch for NinjaStashOverview
        ninjaClientMock.Setup(x => x.Fetch<NinjaStashOverview>(It.IsAny<GameType>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string?>>()))
            .Returns<GameType, string, Dictionary<string, string?>>(async (game, path, parameters) =>
            {
                var type = parameters?["type"];
                if (type == null) return null;
                var dataProvider = services.BuildServiceProvider().GetRequiredService<RawDataProvider>();
                var filePath = $"{game.GetValueAttribute()}/raw/ninja/{type}.json";
                return await dataProvider.Read<NinjaStashOverview>(filePath);
            });

        services.AddSingleton(ninjaClientMock.Object);
    }
}

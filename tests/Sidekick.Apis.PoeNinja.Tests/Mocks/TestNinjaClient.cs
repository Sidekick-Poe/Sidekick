using System.Collections.Generic;
using System.Threading.Tasks;
using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Common.Enums;
using Sidekick.Data;
using Sidekick.Data.Builder;

namespace Sidekick.Apis.PoeNinja.Tests.Mocks;

public class TestNinjaClient(RawDataProvider dataProvider) : INinjaClient
{
    public async Task<TResponse?> Fetch<TResponse>(GameType game, string path, Dictionary<string, string?>? parameters = null)
        where TResponse : class
    {
        if (parameters == null || !parameters.TryGetValue("type", out var type) || type == null)
        {
            return null;
        }

        var filePath = $"{game.GetValueAttribute()}/raw/ninja/{type}.json";
        return await dataProvider.Read<TResponse>(filePath);
    }
}

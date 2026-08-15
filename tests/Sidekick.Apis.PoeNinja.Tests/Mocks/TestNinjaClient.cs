using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sidekick.Apis.PoeNinja.Clients;
using Sidekick.Game;

namespace Sidekick.Apis.PoeNinja.Tests.Mocks;

public class TestNinjaClient : INinjaClient
{
    public async Task<TResponse?> Fetch<TResponse>(GameType game, string path, Dictionary<string, string?>? parameters = null)
        where TResponse : class
    {
        throw new NotImplementedException();
    }
}

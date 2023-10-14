using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sidekick.Apis.Poe.Clients
{
    public interface IPoeApiClient
    {
        JsonSerializerOptions Options { get; }

        Task<TReturn> Fetch<TReturn>(string path);

    }
}

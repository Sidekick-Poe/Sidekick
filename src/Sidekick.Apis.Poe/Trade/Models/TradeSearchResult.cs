using System.Collections.Generic;
using Sidekick.Apis.Poe.Trade.Results;

namespace Sidekick.Apis.Poe.Trade.Models
{
    public class TradeSearchResult<T>
    {
        public List<T> Result { get; set; }

        public string Id { get; set; }

        public int Total { get; set; }

        public Error Error { get; set; }
    }
}

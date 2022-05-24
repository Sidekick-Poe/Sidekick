using System.Collections.Generic;

namespace Sidekick.Apis.PoeWiki.ApiModels
{
    public class CargoQueryResult<T> where T : class
    {
        public List<CargoQuery<T>> CargoQuery { get; set; }
    }

    public class CargoQuery<T> where T : class
    {
        public T Title { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidekick.Apis.PoeWiki
{
    public interface IPoeWikiDataProvider
    {
        Task Initialize();
        Task<Dictionary<string, string>> GetBlightOils();
    }
}

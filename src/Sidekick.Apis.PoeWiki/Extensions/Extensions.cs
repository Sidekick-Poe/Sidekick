using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidekick.Apis.PoeWiki.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Single-quoted comma separated.
        /// 'a','b',...'z'
        /// </summary>
        public static string ToQueryString(this List<string> value)
        {
            return string.Join(",", value.Select(x => $"'{x}'"));
        }
    }
}

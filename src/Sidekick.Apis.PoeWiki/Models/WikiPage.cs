using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sidekick.Apis.PoeWiki.ApiModels;

namespace Sidekick.Apis.PoeWiki.Models
{
    public class WikiPage
    {
        public WikiPage(WikiPageResult wikipage)
        {
            Title = wikipage.Parse.Title;
            PageId = wikipage.Parse.PageId;
            WikiText = wikipage.Parse.WikiText;
        }

        public string Title { get; set; }
        public int PageId { get; set; }
        public string WikiText { get; set; }
    }
}

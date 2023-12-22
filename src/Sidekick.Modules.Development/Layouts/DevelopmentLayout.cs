using System.Threading.Tasks;
using Sidekick.Common.Blazor.Layouts;

namespace Sidekick.Modules.Development.Layouts
{
    public class DevelopmentLayout : MenuLayout
    {
        /// <inheritdoc/>
        protected override async Task OnInitializedAsync()
        {
            MenuLinks = new()
            {
                new()
                {
                    Name= "Typography",
                    Url="/development/typography",
                },
                new()
                {
                    Name= "Tests",
                    Url="/development/tests",
                },
            };

            MenuIcon = false;

            await base.OnInitializedAsync();
        }
    }
}

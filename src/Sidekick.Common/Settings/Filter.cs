namespace Sidekick.Common.Settings;
public class Filter
{
    public FilterType FilterType { get; set; }
    public string? Icon { get; set; }
}

public class Filters
{
    public static List<Filter> All =>
        [
            new Filter {
                FilterType = FilterType.Minimum,
                Icon = @"<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 24 24""><path d=""M6.5,2.27L20,10.14L6.5,18L5.5,16.27L16.03,10.14L5.5,4L6.5,2.27M20,20V22H5V20H20Z"" /></svg>"
            },
            new Filter {
                FilterType = FilterType.Maximum,
                Icon = @"<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 24 24""><path d=""M18.5,2.27L5,10.14L18.5,18L19.5,16.27L8.97,10.14L19.5,4L18.5,2.27M5,20V22H20V20H5Z"" /></svg>"
            },
            new Filter {
                FilterType = FilterType.Equals,
                Icon = @"<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 24 24""><path d=""M19,10H5V8H19V10M19,16H5V14H19V16Z"" /></svg>"
            },
            new Filter {
                FilterType = FilterType.Range,
                Icon = @"<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 24 24""><path d=""M8,3A2,2 0 0,0 6,5V9A2,2 0 0,1 4,11H3V13H4A2,2 0 0,1 6,15V19A2,2 0 0,0 8,21H10V19H8V14A2,2 0 0,0 6,12A2,2 0 0,0 8,10V5H10V3M16,3A2,2 0 0,1 18,5V9A2,2 0 0,0 20,11H21V13H20A2,2 0 0,0 18,15V19A2,2 0 0,1 16,21H14V19H16V14A2,2 0 0,1 18,12A2,2 0 0,1 16,10V5H14V3H16Z"" /></svg>"
            }
        ];
}

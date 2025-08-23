using ApexCharts;

namespace Sidekick.Modules.Wealth.Graphs;

public static class DataConstants
{
    public static ApexChartOptions<DataPoint> GetOptions() => new()
    {
        Chart = new Chart
        {
            Background = "transparent",
            FontFamily = "fontin",
            Toolbar = new Toolbar { Show = false },
            Sparkline = new ChartSparkline
            {
                Enabled = true,
            }
        },
        Theme = new Theme
        {
            Mode = Mode.Dark,
            Palette = PaletteType.Palette8,
        },
        DataLabels = new DataLabels
        {
            Enabled = false,
        },
        Legend = new Legend
        {
            Show = false,
        },
        Yaxis =
        [
            new YAxis
            {
                Show = false,
            },
        ],
        Xaxis = new XAxis
        {
            Labels = new XAxisLabels
            {
                Show = false,
            },
            AxisTicks = new AxisTicks
            {
                Show = false,
                Color = "#666666",
            },
        },
        Grid = new Grid
        {
            BorderColor = "#666666",
            Xaxis = new GridXAxis
            {
                Lines = new Lines
                {
                    Show = false,
                },
            },
            Yaxis = new GridYAxis
            {
                Lines = new Lines
                {
                    Show = true,
                },
            },
        },
        Tooltip = new Tooltip
        {
            FollowCursor = true,
            Y = new TooltipY
            {
                Formatter = @"
                      function (value) {
                        if (!value) return null;
                        return parseInt(value);
                      }",
            },
        },
    };
}

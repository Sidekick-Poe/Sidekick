using ApexCharts;
namespace Sidekick.Modules.Item.Poe2Scout;

public static class DataConstants
{
    private const string TwoDecimalFormatter = @"
                      function (value) {
                        if (!value) return '';
                        if (value % 1 === 0) return value;
                        return value.toFixed(2);
                      }";

    private const string IntFormatter = @"
                      function (value) {
                        if (!value) return '';
                        return parseInt(value);
                      }";

    public static ApexChartOptions<DataPoint> GetOptions() => new()
    {
        Chart = new Chart
        {
            Background = "transparent",
            FontFamily = "fontin",
            Toolbar = new Toolbar
            {
                Show = false
            },
            Zoom = new Zoom
            {
                Enabled = false,
            },
        },
        Legend = new Legend()
        {
            Show = false,
        },
        Theme = new Theme
        {
            Mode = Mode.Dark,
            Palette = PaletteType.Palette8,
        },
        Tooltip = new Tooltip
        {
            FollowCursor = true,
            Y = new TooltipY
            {
                Formatter = TwoDecimalFormatter,
            },
        },
        Xaxis = new XAxis
        {
            AxisBorder = new AxisBorder()
            {
                Show = false,
            },
            AxisTicks = new AxisTicks()
            {
                Show = false,
            },
            Labels = new XAxisLabels()
            {
                Show = false,
            },
            Tooltip = new XAxisTooltip()
            {
                Enabled = false,
            },
        },
        Yaxis =
        [
            new YAxis()
            {
                Labels = new YAxisLabels()
                {
                    Show = true,
                    Formatter = IntFormatter,
                },
            },
            new YAxis()
            {
                Opposite = true,
                Labels = new YAxisLabels()
                {
                    Show = true,
                    Formatter = TwoDecimalFormatter,
                },
            },
        ],
    };
}

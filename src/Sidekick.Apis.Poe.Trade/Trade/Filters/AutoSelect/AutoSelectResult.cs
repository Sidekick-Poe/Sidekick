namespace Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;

public class AutoSelectResult
{
    public bool? Checked { get; set; }

    public bool FillMinRange { get; set; }

    public bool FillMaxRange { get; set; }

    public double NormalizeBy { get; set; }
}

namespace Sidekick.Game.Parser.Filters.AutoSelect;

public class AutoSelectResult
{
    public bool? Checked { get; set; }

    public bool FillMinRange { get; set; }

    public bool FillMaxRange { get; set; }

    public double NormalizeBy { get; set; }

    public bool SelectCategory { get; set; }
}

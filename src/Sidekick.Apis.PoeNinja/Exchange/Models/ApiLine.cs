namespace Sidekick.Apis.PoeNinja.Exchange.Models;

public class ApiLine
{
    public string? Id { get; set; }
    public string? MaxVolumeCurrency { get; set; }
    public decimal MaxVolumeRate { get; set; }
    public decimal PrimaryValue { get; set; }
    public decimal VolumePrimaryValue { get; set; }
    public ApiSparkline? Sparkline { get; set; }
}

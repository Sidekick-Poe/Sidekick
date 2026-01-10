using System.Text.Json.Serialization;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;

#pragma warning disable CS0659

public class AutoSelectRule : IEquatable<AutoSelectRule>
{
    [JsonIgnore]
    public Guid Id { get; } = Guid.NewGuid();

    [JsonPropertyName("checked")]
    public bool? Checked { get; set; }

    [JsonPropertyName("conditions")]
    public List<AutoSelectCondition> Conditions { get; set; } = [];

    public bool Equals(AutoSelectRule? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Checked == other.Checked && Conditions.SequenceEqual(other.Conditions);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((AutoSelectRule)obj);
    }

}

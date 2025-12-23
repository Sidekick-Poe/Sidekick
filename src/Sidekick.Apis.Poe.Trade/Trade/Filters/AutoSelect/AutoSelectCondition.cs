using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Serialize.Linq.Serializers;
using Sidekick.Apis.Poe.Items;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;

#pragma warning disable CS0659

public class AutoSelectCondition : IEquatable<AutoSelectCondition>
{
    private static ExpressionSerializer _expressionSerializer = new ExpressionSerializer(new JsonSerializer());

    [JsonPropertyName("expression")]
    public string ExpressionAsString
    {
        get { return _expressionSerializer.SerializeText(Expression); }
        set { Expression = _expressionSerializer.DeserializeText(value) as Expression<Func<Item, object?>>; }
    }

    [JsonIgnore]
    public Expression<Func<Item, object?>>? Expression { get; set; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AutoSelectConditionType Type { get; set; }

    [JsonPropertyName("value")]
    public object? Value { get; set; }

    public bool Equals(AutoSelectCondition? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Type == other.Type && Equals(Value, other.Value) && ExpressionAsString == other.ExpressionAsString;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((AutoSelectCondition)obj);
    }
}

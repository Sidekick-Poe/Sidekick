using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Serialize.Linq.Serializers;
using Sidekick.Apis.Poe.Items;
namespace Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;

public class AutoSelectCondition
{
    private static ExpressionSerializer _expressionSerializer = new ExpressionSerializer(new JsonSerializer());

    [JsonPropertyName("type")]
    public string TypeAsString
    {
        get { return Type.ToString(); }
        set { Type = Enum.Parse<AutoSelectConditionType>(value); }
    }

    [JsonPropertyName("expression")]
    public string ExpressionAsString
    {
        get { return _expressionSerializer.SerializeText(Expression); }
        set { Expression = _expressionSerializer.DeserializeText(value) as Expression<Func<Item, object>>; }
    }

    [JsonIgnore]
    public Expression<Func<Item, object>>? Expression { get; set; }

    [JsonIgnore]
    public AutoSelectConditionType Type { get; set; }

    [JsonPropertyName("value")]
    public object? Value { get; set; }
}

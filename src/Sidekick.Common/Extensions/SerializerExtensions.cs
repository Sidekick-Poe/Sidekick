using System.Text.Json;

namespace Sidekick.Common.Extensions;

public static class SerializerExtensions
{
    public static string ToJson<T>(this T value, JsonSerializerOptions? jsonSerializerOptions = null) =>
        JsonSerializer.Serialize(value, options: jsonSerializerOptions);

    public static async Task ToJson<T>(this FileStream stream, T data) =>
        await JsonSerializer.SerializeAsync(stream, data);

    public static T? FromJsonTo<T>(this string value, JsonSerializerOptions? jsonSerializerOptions = null) =>
        JsonSerializer.Deserialize<T>(value, options: jsonSerializerOptions);

    public static async Task<T?> FromJsonToAsync<T>(this Stream stream, JsonSerializerOptions? jsonSerializerOptions = null) => 
        await JsonSerializer.DeserializeAsync<T>(stream, options: jsonSerializerOptions);
}

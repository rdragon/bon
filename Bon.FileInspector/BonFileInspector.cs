using System.Text.Json;
using System.Text.Json.Nodes;

namespace Bon.FileInspector;

internal static class BonFileInspector
{
    public static async Task BonToJson(Stream inputStream, Stream outputStream, BonSerializer bonSerializer)
    {
        var jsonObject = await bonSerializer.BonToJsonAsync(inputStream);
        using var writer = new Utf8JsonWriter(outputStream);
        jsonObject.WriteTo(writer);
    }

    public static async Task JsonToBon(Stream inputStream, Stream outputStream, BonSerializer bonSerializer)
    {
        var jsonObject =
            await JsonSerializer.DeserializeAsync<JsonObject>(inputStream) ??
            throw new InvalidOperationException("Failed to deserialize JSON object.");

        await bonSerializer.JsonToBonAsync(outputStream, jsonObject);
    }
}

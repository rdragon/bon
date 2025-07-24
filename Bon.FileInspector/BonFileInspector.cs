using System.Text.Json;
using System.Text.Json.Nodes;

namespace Bon.FileInspector;

internal static class BonFileInspector
{
    public static void BonToJson(Stream inputStream, Stream outputStream, BonSerializer bonSerializer)
    {
        var jsonObject = bonSerializer.BonToJson(inputStream);
        using var writer = new Utf8JsonWriter(outputStream);
        jsonObject.WriteTo(writer);
    }

    public static void JsonToBon(Stream inputStream, Stream outputStream, BonSerializer bonSerializer)
    {
        var jsonObject =
            JsonSerializer.Deserialize<JsonObject>(inputStream) ??
            throw new InvalidOperationException("Failed to deserialize JSON object.");

        bonSerializer.JsonToBon(outputStream, jsonObject);
    }
}

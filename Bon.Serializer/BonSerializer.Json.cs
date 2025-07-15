using System.Text.Json.Nodes;


namespace Bon.Serializer;

partial class BonSerializer
{
    /// <summary>
    /// Deserializes a value from a stream and then converts the value to a <see cref="JsonObject"/>.
    /// Loads new schemas from the storage if unknown schema IDs are encountered.
    /// </summary>
    public async Task<JsonObject> BonToJsonAsync(Stream stream)
    {
        var reader = new BinaryReader(stream);
        var (_, blockId, schemaData) = await ReadHeaderAsync(reader).ConfigureAwait(false);
        var schema = _schemaDataResolver.GetSchemaBySchemaData(schemaData);

        var result = new JsonObject
        {
            ["body"] = BonToJsonDeserializer.Deserialize(reader, schema),
            ["schema"] = SchemaJsonSerializer.Serialize(schemaData),
        };

        if (blockId != 0)
        {
            result["blockId"] = blockId;
        }

        return result;
    }

    /// <summary>
    /// Deserializes a value from a stream and then converts the value to a JSON string.
    /// Loads new schemas from the storage if unknown schema IDs are encountered.
    /// </summary>
    public async Task<string> BonToJsonAsync(byte[] bytes)
    {
        var stream = new MemoryStream(bytes);
        var jsonObject = await BonToJsonAsync(stream).ConfigureAwait(false);
        return jsonObject.ToJsonString();
    }

    /// <summary>
    /// The reverse of <see cref="BonToJsonAsync(Stream)"/>.
    /// </summary>
    public async Task JsonToBonAsync(Stream stream, JsonObject jsonObject)
    {
        var blockId = jsonObject["blockId"]?.GetValue<uint>() ?? 0;
        var schemaData = SchemaJsonSerializer.Deserialize(jsonObject.GetPropertyValue("schema"));
        var body = jsonObject["body"];

        if (blockId != 0)
        {
            await LoadBlock(blockId).ConfigureAwait(false);
        }

        var schema = _schemaDataResolver.GetSchemaBySchemaData(schemaData);
        var writer = new BinaryWriter(stream);

        WriteHeader(writer, blockId, schemaData);
        JsonToBonSerializer.Serialize(writer, body, schema);
    }

    /// <summary>
    /// The reverse of <see cref="BonToJsonAsync(string)"/>.
    /// </summary>
    public async Task<byte[]> JsonToBonAsync(string json)
    {
        var stream = new MemoryStream();
        var jsonNode = JsonNode.Parse(json) ?? throw new ArgumentException("Parsing returned null");
        await JsonToBonAsync(stream, jsonNode.AsObject());

        return stream.ToArray();
    }
}

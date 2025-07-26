using System.Text.Json.Nodes;


namespace Bon.Serializer;

partial class BonSerializer
{
    /// <summary>
    /// Deserializes a value from a stream and then converts the value to a <see cref="JsonObject"/>.
    /// If unknown schema IDs are encountered then an exception is thrown.
    /// </summary>
    public JsonObject BonToJson(Stream stream)
    {
        var reader = new BinaryReader(stream);
        var schema = ReadSchema(reader);

        var result = new JsonObject
        {
            ["body"] = BonToJsonDeserializer.Deserialize(reader, schema),
            ["schema"] = SchemaJsonSerializer.Write(schema),
        };

        return result;
    }

    /// <summary>
    /// Deserializes a value from a stream and then converts the value to a JSON string.
    /// Loads new schemas from the storage if unknown schema IDs are encountered.
    /// </summary>
    public string BonToJson(byte[] bytes)
    {
        var stream = new MemoryStream(bytes);
        var jsonObject = BonToJson(stream);
        return jsonObject.ToJsonString();
    }

    /// <summary>
    /// The reverse of <see cref="BonToJson"/>.
    /// If unknown schema IDs are encountered then an exception is thrown.
    /// </summary>
    public void JsonToBon(Stream stream, JsonObject jsonObject)
    {
        var schema = new SchemaJsonSerializer(_layoutStore).Read(jsonObject.GetPropertyValue("schema"));
        var body = jsonObject["body"];
        var writer = new BinaryWriter(stream);
        WriteSchema(writer, schema);
        JsonToBonSerializer.Serialize(writer, body, schema);
    }

    /// <summary>
    /// The reverse of <see cref="BonToJson"/>.
    /// </summary>
    public byte[] JsonToBon(string json)
    {
        var stream = new MemoryStream();
        var jsonNode = JsonNode.Parse(json) ?? throw new ArgumentException("Parsing returned null");
        JsonToBon(stream, jsonNode.AsObject());

        return stream.ToArray();
    }
}

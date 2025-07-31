using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Bon.Serializer.Schemas;

/// <summary>
/// Provides methods for serializing a <see cref="Schema"/> to JSON and deserializing it back.
/// </summary>
internal sealed class SchemaJsonSerializer(LayoutStore layoutStore)
{
    private static JsonSerializerOptions? _options;

    public static JsonNode? Write(Schema schema) => JsonSerializer.SerializeToNode(Convert(schema), Options);

    public Schema Read(JsonNode? jsonNode) => Convert(ReadNow(jsonNode));

    private static SerializableSchema ReadNow(JsonNode? jsonNode) =>
        jsonNode?.Deserialize<SerializableSchema>(Options) ?? throw new InvalidOperationException("Invalid JSON");

    private Schema Convert(SerializableSchema serializableSchema)
    {
        var schemaArguments = serializableSchema.SchemaArguments?.Select(Convert).ToArray();
        var members = GetMembers(serializableSchema);
        return Schema.Create(serializableSchema.SchemaType, schemaArguments, serializableSchema.LayoutId ?? 0, members);
    }

    private IReadOnlyList<SchemaMember> GetMembers(SerializableSchema serializableSchema)
    {
        if (!serializableSchema.SchemaType.IsCustomSchema())
        {
            return [];
        }

        return layoutStore.GetLayout(serializableSchema.LayoutId ?? 0).Members;
    }

    private static SerializableSchema Convert(Schema schema)
    {
        var layoutId = schema.LayoutId > 0 ? schema.LayoutId : (int?)null;
        var schemaArguments = schema.SchemaArguments.Count == 0 ? null : schema.SchemaArguments.Select(Convert).ToArray();

        return new SerializableSchema(schema.SchemaType, schemaArguments, layoutId);
    }

    private static JsonSerializerOptions Options => _options ??= new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter<SchemaType>() }
    };

    // A class that can be serialized to JSON without a custom converter.
    public sealed record class SerializableSchema(
         SchemaType SchemaType,
         IReadOnlyList<SerializableSchema>? SchemaArguments,
         int? LayoutId);
}

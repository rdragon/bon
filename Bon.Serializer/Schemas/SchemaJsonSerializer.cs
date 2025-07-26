using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Bon.Serializer.Schemas;

/// <summary>
/// Provides methods for serializing a <see cref="Schema"/> to JSON and deserializing it back.
/// </summary>
internal class SchemaJsonSerializer(LayoutStore layoutStore)
{
    private static JsonSerializerOptions? _options;

    public static JsonNode? Write(Schema schema) => JsonSerializer.SerializeToNode(Convert(schema), Options);

    public Schema Read(JsonNode? jsonNode) => Convert(ReadNow(jsonNode));

    private static SerializableSchema ReadNow(JsonNode? jsonNode) =>
        jsonNode?.Deserialize<SerializableSchema>(Options) ?? throw new InvalidOperationException("Invalid JSON");

    private Schema Convert(SerializableSchema serializableSchema)
    {
        var innerSchemas = serializableSchema.InnerSchemas?.Select(Convert).ToArray();
        var members = GetMembers(serializableSchema);
        return Schema.Create(serializableSchema.SchemaType, innerSchemas, serializableSchema.LayoutId ?? 0, members);
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
        var innerSchemas = schema.InnerSchemas.Count == 0 ? null : schema.InnerSchemas.Select(Convert).ToArray();

        return new SerializableSchema(schema.SchemaType, innerSchemas, layoutId);
    }

    private static JsonSerializerOptions Options => _options ??= new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter<SchemaType>() }
    };

    // A class that can be serialized to JSON without a custom converter.
    public sealed record class SerializableSchema(
         SchemaType SchemaType,
         IReadOnlyList<SerializableSchema>? InnerSchemas,
         int? LayoutId);
}

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Bon.Serializer.Schemas;

/// <summary>
/// Provides methods for serializing a <see cref="SchemaData"/> to JSON and deserializing it back.
/// </summary>
internal static class SchemaJsonSerializer
{
    private static JsonSerializerOptions? _options;

    public static JsonNode? Serialize(SchemaData schemaData) =>
        JsonSerializer.SerializeToNode(ExplicitSchemaData.FromSchemaData(schemaData), Options);

    public static SchemaData Deserialize(JsonNode? jsonNode) =>
        jsonNode?.Deserialize<ExplicitSchemaData>(Options)?.ToSchemaData() ??
        throw new InvalidOperationException("Invalid JSON");

    private static JsonSerializerOptions Options => _options ??= new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter<SchemaType>() }
    };

    // A class that can be serialized to JSON without a custom converter.
    public sealed record class ExplicitSchemaData(
         SchemaType SchemaType,
         int? LayoutId,
         IReadOnlyList<ExplicitSchemaData>? InnerSchemas)
    {
        public SchemaData ToSchemaData()
        {
            if (LayoutId is null)
            {
                return new SchemaData(SchemaType, InnerSchemas?.Select(s => s.ToSchemaData()).ToArray() ?? []);
            }

            return new CustomSchemaData(SchemaType, LayoutId.Value);
        }

        public static ExplicitSchemaData FromSchemaData(SchemaData schemaData)
        {
            var layoutId = (schemaData as CustomSchemaData)?.LayoutId;
            var innerSchemas = schemaData.InnerSchemas.Count == 0 ? null : schemaData.InnerSchemas.Select(FromSchemaData).ToArray();

            return new ExplicitSchemaData(schemaData.SchemaType, layoutId, innerSchemas);
        }
    }
}

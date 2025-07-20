namespace Bon.Serializer.Schemas;

/// <summary>
/// Represents a generic type with one type parameter.
/// </summary>
public abstract class GenericSchema1(SchemaType schemaType) : Schema(schemaType)
{
    public Schema InnerSchema { get; set; } = null!;

    public override IEnumerable<Schema> GetInnerSchemas() => [InnerSchema];

    /// <summary>
    /// Used by source generated code.
    /// </summary>
    public void SetInnerSchemas(Schema innerSchema) => InnerSchema = innerSchema;

    public override void AppendHashCode(Dictionary<Schema, int> ancestors, ref HashCode hashCode)
    {
        hashCode.Add(SchemaType);
        InnerSchema.AppendHashCode(ancestors, ref hashCode);
    }

    public override bool Equals(object? obj, Dictionary<Ancestor, int> ancestors)
    {
        return
            obj is GenericSchema1 other &&
            SchemaType == other.SchemaType &&
            InnerSchema.Equals(other.InnerSchema, ancestors);
    }

    /// <summary>
    /// Used by source generated code.
    /// </summary>
    public static GenericSchema1 Create(SchemaType schemaType)
    {
        GenericSchema1 schema = schemaType switch
        {
            SchemaType.Array => new ArraySchema(),
        };

        return schema;
    }
}

/// <summary>
/// Represents an array, list or enumerable.
/// </summary>
public sealed class ArraySchema() : GenericSchema1(SchemaType.Array)
{
    // Used by source generated code.
    public static ArraySchema ByteArray => new() { InnerSchema = NativeSchema.Byte };
}

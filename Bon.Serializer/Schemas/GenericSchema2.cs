namespace Bon.Serializer.Schemas;

/// <summary>
/// Represents a generic type with two type parameters.
/// </summary>
public abstract class GenericSchema2(SchemaType SchemaType) : Schema(SchemaType)
{
    public Schema InnerSchema1 { get; set; } = null!;
    public Schema InnerSchema2 { get; set; } = null!;

    public override IEnumerable<Schema> GetInnerSchemas() => [InnerSchema1, InnerSchema2];

    /// <summary>
    /// Used by source generated code.
    /// </summary>
    public void SetInnerSchemas(Schema innerSchema1, Schema innerSchema2)
    {
        InnerSchema1 = innerSchema1;
        InnerSchema2 = innerSchema2;
    }

    public override void AppendHashCode(Dictionary<Schema, int> ancestors, ref HashCode hashCode)
    {
        hashCode.Add(SchemaType);
        InnerSchema1.AppendHashCode(ancestors, ref hashCode);
        InnerSchema2.AppendHashCode(ancestors, ref hashCode);
    }

    public override bool Equals(object? obj, Dictionary<Ancestor, int> ancestors)
    {
        return
            obj is GenericSchema2 other &&
            SchemaType == other.SchemaType &&
            InnerSchema1.Equals(other.InnerSchema1, ancestors) &&
            InnerSchema2.Equals(other.InnerSchema2, ancestors);
    }

    /// <summary>
    /// Used by source generated code.
    /// </summary>
    public static GenericSchema2 Create(SchemaType schemaType)
    {
        GenericSchema2 schema = schemaType switch
        {
            SchemaType.Dictionary => new DictionarySchema(schemaType),
            SchemaType.Tuple2 or SchemaType.NullableTuple2 => new Tuple2Schema(schemaType),
        };

        return schema;
    }
}

/// <summary>
/// Represents a dictionary.
/// </summary>
public sealed class DictionarySchema(SchemaType schemaType) : GenericSchema2(schemaType);

/// <summary>
/// Represents a value tuple with two elements.
/// </summary>
public sealed class Tuple2Schema(SchemaType schemaType) : GenericSchema2(schemaType);

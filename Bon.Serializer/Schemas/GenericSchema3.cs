namespace Bon.Serializer.Schemas;

/// <summary>
/// Represents a generic type with three type parameters.
/// </summary>
public abstract class GenericSchema3(SchemaType SchemaType) : Schema(SchemaType)
{
    public Schema InnerSchema1 { get; set; } = null!;
    public Schema InnerSchema2 { get; set; } = null!;
    public Schema InnerSchema3 { get; set; } = null!;

    public override IEnumerable<Schema> GetInnerSchemas() => [InnerSchema1, InnerSchema2, InnerSchema3];

    /// <summary>
    /// Used by source generated code.
    /// </summary>
    public void SetInnerSchemas(Schema innerSchema1, Schema innerSchema2, Schema innerSchema3)
    {
        InnerSchema1 = innerSchema1;
        InnerSchema2 = innerSchema2;
        InnerSchema3 = innerSchema3;
    }

    public override void AppendHashCode(Dictionary<Schema, int> ancestors, ref HashCode hashCode)
    {
        hashCode.Add(SchemaType);
        InnerSchema1.AppendHashCode(ancestors, ref hashCode);
        InnerSchema2.AppendHashCode(ancestors, ref hashCode);
        InnerSchema3.AppendHashCode(ancestors, ref hashCode);
    }

    public override bool Equals(object? obj, Dictionary<Ancestor, int> ancestors)
    {
        return
            obj is GenericSchema3 other &&
            SchemaType == other.SchemaType &&
            InnerSchema1.Equals(other.InnerSchema1, ancestors) &&
            InnerSchema2.Equals(other.InnerSchema2, ancestors) &&
            InnerSchema3.Equals(other.InnerSchema3, ancestors);
    }

    /// <summary>
    /// Used by source generated code.
    /// </summary>
    public static GenericSchema3 Create(SchemaType schemaType)
    {
        GenericSchema3 schema = schemaType switch
        {
            SchemaType.Tuple3 or SchemaType.NullableTuple3 => new Tuple3Schema(schemaType),
        };

        return schema;
    }
}

/// <summary>
/// Represents a value tuple with three elements.
/// </summary>
public sealed class Tuple3Schema(SchemaType schemaType) : GenericSchema3(schemaType);

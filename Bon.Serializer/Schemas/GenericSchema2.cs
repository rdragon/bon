namespace Bon.Serializer.Schemas;

/// <summary>
/// Represents a generic type with two type parameters.
/// </summary>
public abstract class GenericSchema2(SchemaType SchemaType, bool IsNullable) : Schema(SchemaType, IsNullable)
{
    public Schema InnerSchema1 { get; set; } = null!;
    public Schema InnerSchema2 { get; set; } = null!;

    public override IEnumerable<Schema> GetInnerSchemas() => [InnerSchema1, InnerSchema2];

    public void SetInnerSchemas(Schema innerSchema1, Schema innerSchema2)
    {
        InnerSchema1 = innerSchema1;
        InnerSchema2 = innerSchema2;
    }

    public override void AppendHashCode(Dictionary<Schema, int> ancestors, ref HashCode hashCode)
    {
        hashCode.Add(SchemaType);
        hashCode.Add(IsNullable);
        InnerSchema1.AppendHashCode(ancestors, ref hashCode);
        InnerSchema2.AppendHashCode(ancestors, ref hashCode);
    }

    public override bool Equals(object? obj, Dictionary<Ancestor, int> ancestors)
    {
        return
            obj is GenericSchema2 other &&
            SchemaType == other.SchemaType &&
            IsNullable == other.IsNullable &&
            InnerSchema1.Equals(other.InnerSchema1, ancestors) &&
            InnerSchema2.Equals(other.InnerSchema2, ancestors);
    }

    public static GenericSchema2 Create(SchemaType schemaType, bool isNullable)
    {
        GenericSchema2 schema = schemaType switch
        {
            SchemaType.Dictionary => new DictionarySchema(schemaType, isNullable),
            SchemaType.Tuple2 => new Tuple2Schema(schemaType, isNullable),
            _ => throw new ArgumentOutOfRangeException(nameof(schemaType), schemaType, null),
        };

        return schema;
    }
}

/// <summary>
/// Represents a dictionary.
/// </summary>
public sealed class DictionarySchema(SchemaType schemaType, bool isNullable) : GenericSchema2(schemaType, isNullable);

/// <summary>
/// Represents a value tuple with two elements.
/// </summary>
public sealed class Tuple2Schema(SchemaType schemaType, bool isNullable) : GenericSchema2(schemaType, isNullable);

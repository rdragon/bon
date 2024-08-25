namespace Bon.Serializer.Schemas;

/// <summary>
/// Represents a generic type with one type parameter.
/// </summary>
public abstract class GenericSchema1(SchemaType schemaType, bool isNullable) : Schema(schemaType, isNullable)
{
    public Schema InnerSchema { get; set; } = null!;

    public override IEnumerable<Schema> GetInnerSchemas() => [InnerSchema];

    public void SetInnerSchemas(Schema innerSchema) => InnerSchema = innerSchema;

    public override void AppendHashCode(Dictionary<Schema, int> ancestors, ref HashCode hashCode)
    {
        hashCode.Add(SchemaType);
        hashCode.Add(IsNullable);
        InnerSchema.AppendHashCode(ancestors, ref hashCode);
    }

    public override bool Equals(object? obj, Dictionary<Ancestor, int> ancestors)
    {
        return
            obj is GenericSchema1 other &&
            SchemaType == other.SchemaType &&
            IsNullable == other.IsNullable &&
            InnerSchema.Equals(other.InnerSchema, ancestors);
    }

    public static GenericSchema1 Create(SchemaType schemaType, bool isNullable)
    {
        GenericSchema1 schema = schemaType switch
        {
            SchemaType.Array => new ArraySchema(schemaType, isNullable),
            _ => throw new ArgumentOutOfRangeException(nameof(schemaType), schemaType, null),
        };

        return schema;
    }
}

/// <summary>
/// Represents an array, list or enumerable.
/// </summary>
public sealed class ArraySchema(SchemaType schemaType, bool isNullable) : GenericSchema1(schemaType, isNullable);

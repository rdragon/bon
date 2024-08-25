namespace Bon.Serializer.Schemas;

/// <summary>
/// Determines the format in which a value is serialized.
/// During deserialization the schema is used to give meaning to the bytes which are read.
/// 
/// There is a many-to-many relationship between schemas and types.
/// 
/// In general types that have the same structure (e.g. members of the same types with the same IDs) will share the same schema.
/// As a schema can map to more than one type, during deserialization the schema doesn't give enough information to determine the
/// type of the resulting value.
/// 
/// Some types have more than one schema.
/// This is only the case for reference types or types that contain at least one reference type (e.g. as member or type argument).
/// The reason is that a reference type does not have nullability information during runtime, while a schema has.
/// For example, during runtime string and string? are the same type, but they have different schemas.
/// 
/// Enums use as schema the schema of their underlying type.
/// Therefore there are no specific schemas for enums.
/// The same holds for <see cref="char"/> and some other "weak" types (like <see cref="DateTime"/>).
/// </summary>
public abstract class Schema(SchemaType schemaType, bool isNullable)
{
    public SchemaType SchemaType { get; } = schemaType;

    public bool IsNullable { get; } = isNullable;

    public abstract IEnumerable<Schema> GetInnerSchemas();

    public AnnotatedSchemaType AnnotatedSchemaType => new(SchemaType, IsNullable);

    public override int GetHashCode()
    {
        var hashCode = new HashCode();

        AppendHashCode(new Dictionary<Schema, int>(ReferenceEqualityComparer.Instance), ref hashCode);

        return hashCode.ToHashCode();
    }

    public override bool Equals(object? obj) => Equals(obj, new Dictionary<Ancestor, int>(AncestorEqualityComparer.Instance));

    public abstract void AppendHashCode(Dictionary<Schema, int> ancestors, ref HashCode hashCode);

    public abstract bool Equals(object? obj, Dictionary<Ancestor, int> ancestors);

    protected bool? StartEquals<T>(object? obj, Dictionary<Ancestor, int> ancestors, out T other) where T : Schema
    {
        if (obj is not T value)
        {
            other = null!;

            return false;
        }

        other = value;

        if (ancestors.TryGetValue(new(this, true), out var id))
        {
            return ancestors.TryGetValue(new(other, false), out var otherId) && id == otherId;
        }

        if (SchemaType != other.SchemaType ||
            IsNullable != other.IsNullable)
        {
            return false;
        }

        var currentId = ancestors.Count;
        ancestors[new(this, true)] = currentId;
        ancestors[new(other, false)] = currentId;

        return null;
    }

    protected void EndEquals(Schema other, Dictionary<Ancestor, int> ancestors)
    {
        ancestors.Remove(new(this, true));
        ancestors.Remove(new(other, false));
    }

    public static Schema CreateNonCustomSchema(SchemaType schemaType, bool isNullable, params Schema[] innerSchemas)
    {
        return schemaType switch
        {
            SchemaType.Array => new ArraySchema(schemaType, isNullable)
            {
                InnerSchema = innerSchemas[0],
            },

            SchemaType.Dictionary => new DictionarySchema(schemaType, isNullable)
            {
                InnerSchema1 = innerSchemas[0],
                InnerSchema2 = innerSchemas[1],
            },

            SchemaType.Tuple2 => new Tuple2Schema(schemaType, isNullable)
            {
                InnerSchema1 = innerSchemas[0],
                InnerSchema2 = innerSchemas[1],
            },

            SchemaType.Tuple3 => new Tuple3Schema(schemaType, isNullable)
            {
                InnerSchema1 = innerSchemas[0],
                InnerSchema2 = innerSchemas[1],
                InnerSchema3 = innerSchemas[2],
            },

            _ => new AnnotatedSchemaType(schemaType, isNullable).ToNativeSchema(),
        };
    }
}

public readonly record struct Ancestor(Schema Schema, bool MyAncestor);

public sealed class AncestorEqualityComparer : IEqualityComparer<Ancestor>
{
    public static AncestorEqualityComparer Instance { get; } = new();

    public bool Equals(Ancestor x, Ancestor y) => ReferenceEquals(x.Schema, y.Schema) && x.MyAncestor == y.MyAncestor;

    public int GetHashCode(Ancestor obj) => HashCode.Combine(ReferenceEqualityComparer.Instance.GetHashCode(obj.Schema), obj.MyAncestor);
}

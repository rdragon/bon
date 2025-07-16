namespace Bon.Serializer.Schemas;

/// <summary>
/// Determines the format in which a value is serialized.
/// During deserialization the schema is used to give meaning to the bytes which are read.
/// 
/// Enums use as schema the schema of their underlying type.
/// Therefore there are no specific schemas for enums.
/// The same holds for <see cref="char"/> and some other "weak" types (like <see cref="DateTime"/>).
/// </summary>
public abstract class Schema(SchemaType schemaType)
{
    public SchemaType SchemaType { get; } = schemaType;

    public bool IsNullable { get; } = false;//1at

    public abstract IEnumerable<Schema> GetInnerSchemas();

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

        if (SchemaType != other.SchemaType)
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

    public static Schema CreateNonCustomSchema(SchemaType schemaType, params Schema[] innerSchemas)
    {
        return schemaType switch
        {
            SchemaType.Array => new ArraySchema(schemaType)
            {
                InnerSchema = innerSchemas[0],
            },

            SchemaType.Dictionary => new DictionarySchema(schemaType)
            {
                InnerSchema1 = innerSchemas[0],
                InnerSchema2 = innerSchemas[1],
            },

            SchemaType.Tuple2 => new Tuple2Schema(schemaType)
            {
                InnerSchema1 = innerSchemas[0],
                InnerSchema2 = innerSchemas[1],
            },

            SchemaType.Tuple3 => new Tuple3Schema(schemaType)
            {
                InnerSchema1 = innerSchemas[0],
                InnerSchema2 = innerSchemas[1],
                InnerSchema3 = innerSchemas[2],
            },

            _ => NativeSchema.FromSchemaType(schemaType),
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

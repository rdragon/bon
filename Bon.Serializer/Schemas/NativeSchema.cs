// Bookmark 413211217
namespace Bon.Serializer.Schemas;

/// <summary>
/// Represents a string, bool, byte, sbyte, short, ushort, int, uint, long, ulong, float, double, decimal, or Guid.
/// </summary>
public sealed class NativeSchema(SchemaType schemaType, bool isNullable) : Schema(schemaType, isNullable)
{
    public bool IsReferenceType => SchemaType == SchemaType.String; // Bookmark 413211217

    public bool IsNullableReferenceType => IsNullable && IsReferenceType;

    public override IEnumerable<Schema> GetInnerSchemas() => [];

    public override void AppendHashCode(Dictionary<Schema, int> ancestors, ref HashCode hashCode)
    {
        hashCode.Add(SchemaType);
        hashCode.Add(IsNullable);
    }

    public override bool Equals(object? obj, Dictionary<Ancestor, int> ancestors)
    {
        return obj is NativeSchema other && SchemaType == other.SchemaType && IsNullable == other.IsNullable;
    }

    public static NativeSchema String { get; } = new(SchemaType.String, false);
    public static NativeSchema Bool { get; } = new(SchemaType.Bool, false);
    public static NativeSchema Byte { get; } = new(SchemaType.Byte, false);
    public static NativeSchema SByte { get; } = new(SchemaType.SByte, false);
    public static NativeSchema Short { get; } = new(SchemaType.Short, false);
    public static NativeSchema UShort { get; } = new(SchemaType.UShort, false);
    public static NativeSchema Int { get; } = new(SchemaType.Int, false);
    public static NativeSchema UInt { get; } = new(SchemaType.UInt, false);
    public static NativeSchema Long { get; } = new(SchemaType.Long, false);
    public static NativeSchema ULong { get; } = new(SchemaType.ULong, false);
    public static NativeSchema WholeNumber { get; } = new(SchemaType.WholeNumber, false);
    public static NativeSchema SignedWholeNumber { get; } = new(SchemaType.SignedWholeNumber, false);
    public static NativeSchema Float { get; } = new(SchemaType.Float, false);
    public static NativeSchema Double { get; } = new(SchemaType.Double, false);
    public static NativeSchema Decimal { get; } = new(SchemaType.Decimal, false);
    public static NativeSchema Guid { get; } = new(SchemaType.Guid, false);

    public static NativeSchema NullableString { get; } = new(SchemaType.String, true);
    public static NativeSchema NullableBool { get; } = new(SchemaType.Bool, true);
    public static NativeSchema NullableWholeNumber { get; } = new(SchemaType.WholeNumber, true);
    public static NativeSchema NullableSignedWholeNumber { get; } = new(SchemaType.SignedWholeNumber, true);
    public static NativeSchema NullableFloat { get; } = new(SchemaType.Float, true);
    public static NativeSchema NullableDouble { get; } = new(SchemaType.Double, true);
    public static NativeSchema NullableDecimal { get; } = new(SchemaType.Decimal, true);
    public static NativeSchema NullableGuid { get; } = new(SchemaType.Guid, true);

    public static NativeSchema Create(SchemaType schemaType, bool isNullable)
    {
        return new AnnotatedSchemaType(schemaType, isNullable).ToNativeSchema();
    }
}
